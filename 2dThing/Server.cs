using System;
using System.Threading;
using _2dThing.GameContent;
using _2dThing.Utils;
using Lidgren.Network;
using System.Collections.Generic;
using SFML.Window;

namespace _2dThing
{
	public class Server
	{
		Ticker ticker;
		World map;
		DateTime lastTickTime;
		NetServer server;
		List<NetworkClient> clientList;
		int clientId = 1;
		
		public Server ()
		{
			this.ticker = new Ticker ();
			this.map = new World ();
			this.map.addCube(new Vector2f(0, 90));
			lastTickTime = DateTime.Now;
			NetPeerConfiguration netConfiguration = new NetPeerConfiguration ("2dThing");			
			netConfiguration.Port = 55017;			
			server = new NetServer (netConfiguration);
			clientList = new List<NetworkClient> ();
		}
		
		public void run ()
		{
			server.Start ();
			Console.WriteLine ("Server started");
			while (true) {
				if (ticker.Tick ()) {
					update ((float)(DateTime.Now - lastTickTime).TotalSeconds);
					lastTickTime = DateTime.Now;
				} else {
					Thread.Sleep (10);
				}
			}
			
		}
				
		public void update (float time)
		{
			readIncomingMsg ();			
		}
		
		public void readIncomingMsg ()
		{
			NetIncomingMessage msg;
			while ((msg = server.ReadMessage()) != null) {
				switch (msg.MessageType) {
				case NetIncomingMessageType.VerboseDebugMessage:
				case NetIncomingMessageType.DebugMessage:
				case NetIncomingMessageType.WarningMessage:
				case NetIncomingMessageType.ErrorMessage:
					Console.WriteLine (msg.ReadString ());
					break;
				case NetIncomingMessageType.StatusChanged:
					NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte ();
					if (status == NetConnectionStatus.Connected) {
						int id = getUniqueClientId ();
						NetworkClient newClient = new NetworkClient (id, msg.SenderConnection);
						clientList.Add (newClient);
						Console.WriteLine (newClient.Pseudo + " connected");
							
						
						ClientInfo ci = new ClientInfo (id);
						ci.Pseudo = newClient.Pseudo;
						sendPkt(ci, msg.SenderConnection, true);
						
						Player p = new Player(map);
						map.addPlayer(p);
						newClient.Player = p;
						sendFullWorldUpdate(msg.SenderConnection);
							
						
					} else if (status == NetConnectionStatus.Disconnected) {
						foreach (NetworkClient c in clientList) {
							if (c.Connection == msg.SenderConnection) {
								clientList.Remove (c);
								map.deletePlayer(c.Player);
								Console.WriteLine ("Client " + c.Pseudo + " disconnected");
								
								ClientDisconnect cd = new ClientDisconnect(c.ClientId);
								sendPktToAll(cd, true);
								break;
							}
						}						
					}						
					break;	
					
				case NetIncomingMessageType.Data:
					readPacket (msg);
					break;
				default:
					Console.WriteLine ("Unhandled type: " + msg.MessageType);
					break;
				}
				server.Recycle (msg);
			}
		}
		
		public void readPacket (NetIncomingMessage msg)
		{
			
			switch (msg.PeekByte ()) {				
			case Packet.CLIENTINFO:{
				foreach (NetworkClient c in clientList) {
					if (c.Connection.Equals (msg.SenderConnection)) {
						ClientInfo ci = ClientInfo.decode (ref msg);						
						Console.WriteLine ("Client " + c.Pseudo + " changed pseudo for " + ci.Pseudo);
						c.Pseudo = ci.Pseudo;
					}
				}
				break;
			}
			case Packet.USERMESSAGE:{
				UserMessage uMsg = UserMessage.decode (ref msg);
				foreach(NetworkClient c in clientList){
					if(c.Connection.Equals(msg.SenderConnection)){
					
						
						c.Player.update((float) (uMsg.Time - c.LastUpdate).TotalSeconds, uMsg.Input);
						c.LastUpdate = uMsg.Time;
											
						
						uMsg.Position = c.Player.Position;
						sendPktToAll(uMsg);
					}
				}
				break;
			}
			case Packet.BLOCKUPDATE:{
				BlockUpdate bu = BlockUpdate.decode(ref msg);
				
				if((bu.Added && map.addCube(bu.Position)) || !bu.Added) {
					sendPktToAll(bu);
				}
					
				if(!bu.Added)
					map.deleteCube(bu.Position);
				
				
				break;
			}
			default:
				Console.WriteLine ("Unsupported packet recieved");
				break;
			}
		}
		
		private int getUniqueClientId ()
		{
			return clientId++;
		}
		
		private void sendFullWorldUpdate(NetConnection client){
			
			foreach (Cube c in map.CubeList){
				BlockUpdate bu = new BlockUpdate(0);
				bu.Added = true;
				bu.Position = c.Position;
				sendPkt(bu, client, true);
			}
		}
		
		private void sendPkt(Packet pkt, NetConnection client){
			sendPkt(pkt, client, false);
		}
		
		private void sendPkt(Packet pkt, NetConnection client, bool secure){
			NetOutgoingMessage outMsg = server.CreateMessage();
			pkt.encode(ref outMsg);
			if(secure)
				server.SendMessage(outMsg, client, NetDeliveryMethod.ReliableUnordered);
			else
				server.SendMessage(outMsg, client, NetDeliveryMethod.Unreliable);
		}
		
		private void sendPktToAll(Packet pkt){
			sendPktToAll(pkt, false);
		}
		
		private void sendPktToAll(Packet pkt, bool secure){
			NetOutgoingMessage outMsg = server.CreateMessage();
			pkt.encode(ref outMsg);
			if(secure)
				server.SendToAll(outMsg, NetDeliveryMethod.ReliableUnordered);
			else
				server.SendToAll(outMsg, NetDeliveryMethod.Unreliable);
		}
		
	}
}

