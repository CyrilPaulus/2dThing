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
		Dictionary<NetConnection, NetworkClient> clientList;
		int clientId = 1;
		bool running = false;
		bool local = false;
		ImageManager imageManager;
		
		public Server ()
		{
			imageManager = new ImageManager();
			this.ticker = new Ticker ();
			
			NetPeerConfiguration netConfiguration = new NetPeerConfiguration ("2dThing");			
			netConfiguration.Port = 55017;			
			server = new NetServer (netConfiguration);
			clientList = new Dictionary<NetConnection, NetworkClient>();
		}
		
		public Server (bool local) : this()
		{
			this.local = local;
		}
		
		public void run ()
		{
			running = true;
			
			this.map = new World (imageManager);
			this.map.addCube(new Vector2f(0, 90), 0, World.LAYERNBR - 1);
			
			lastTickTime = DateTime.Now;
			clientList.Clear();
			
			server.Start ();
			Console.WriteLine ("Server started");
			while (running) {
				if (ticker.Tick ()) {
					update ((float)(DateTime.Now - lastTickTime).TotalSeconds);
					lastTickTime = DateTime.Now;
				} else {
					Thread.Sleep (10);
				}
			}			
			
		}
		
		public void stop()
		{
			running = false;
			Console.WriteLine("Shutting server down");
			server.Shutdown("Time to sleep bitches");
			while (server.Status != NetPeerStatus.NotRunning) {				
				Thread.Sleep (10);
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
						clientList.Add (msg.SenderConnection, newClient);
						Console.WriteLine (newClient.Pseudo + " connected");
							
						
						ClientInfo ci = new ClientInfo (id);
						ci.Pseudo = newClient.Pseudo;
						sendPkt(ci, msg.SenderConnection, true);
						
						Player p = new Player(map, imageManager);
						map.addPlayer(p);
						newClient.Player = p;
						sendFullWorldUpdate(msg.SenderConnection);
						sendFullClientInfo(msg.SenderConnection);
							
						
					} else if (status == NetConnectionStatus.Disconnected) {
						if(clientList.ContainsKey(msg.SenderConnection)){						
							NetworkClient c = clientList[msg.SenderConnection];
							clientList.Remove (c.Connection);
							map.deletePlayer(c.Player);
							Console.WriteLine ("Client " + c.Pseudo + " disconnected");
								
							ClientDisconnect cd = new ClientDisconnect(c.ClientId);
							sendPktToAll(cd, true);								
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
			case Packet.CLIENTINFO:				
				if (clientList.ContainsKey(msg.SenderConnection)) {
					NetworkClient c = clientList[msg.SenderConnection];
					ClientInfo ci = ClientInfo.decode (ref msg);						
					Console.WriteLine ("Client " + c.Pseudo + " changed pseudo for " + ci.Pseudo);
					c.Pseudo = ci.Pseudo;
					c.Player.Color = ci.Color;
					sendPktToAll(ci);
				}				
				break;
			
			case Packet.USERMESSAGE:
				UserMessage uMsg = UserMessage.decode (ref msg);
				if (clientList.ContainsKey(msg.SenderConnection)) {
					NetworkClient c = clientList[msg.SenderConnection];
					c.Player.Layer = uMsg.Layer;
					c.Player.Noclip = uMsg.Nolcip;
					c.Player.update((float) (uMsg.Time - c.LastUpdate).TotalSeconds, uMsg.Input);
					c.LastUpdate = uMsg.Time;	
					uMsg.Position = c.Player.Position;
					sendPktToAll(uMsg, true);				
				}
				break;
			
			case Packet.BLOCKUPDATE:				
				BlockUpdate bu = BlockUpdate.decode(ref msg);
				
				if((bu.Added && map.addCube(bu.Position, bu.BlockType, bu.Layer)) || !bu.Added) {
					sendPktToAll(bu);
				}
					
				if(!bu.Added)
					map.deleteCube(bu.Position, bu.Layer);		
				
				break;
			
			case Packet.CLIENTRESET:
				if (clientList.ContainsKey(msg.SenderConnection)) {
						NetworkClient c = clientList[msg.SenderConnection];
						c.Player.reset();
				}
				break;
			case Packet.CHATMESSAGE:
				ChatMessage cm = ChatMessage.decode(ref msg);
				sendPktToAll(cm, true);
				Console.WriteLine(cm.Pseudo +": " + cm.Message);
				break;
			
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
			int layer = 0;
			foreach (List<Cube> cubeList in map.CubeLists){
				foreach (Cube c in cubeList){
					BlockUpdate bu = new BlockUpdate(0);
					bu.Added = true;
					bu.Position = c.Position;
					bu.BlockType = (byte) c.type;
					bu.Layer = (byte) layer;
					sendPkt(bu, client, true);
				}
				layer++;
			}
		}
		
		private void sendFullClientInfo(NetConnection client){
			foreach (NetworkClient c in clientList.Values){
				ClientInfo ci = new ClientInfo(c.ClientId);
				ci.Color = c.Player.Color;
				ci.Pseudo = c.Pseudo;
				sendPkt(ci, client, true);
			}
		}
		
		private void sendPkt(Packet pkt, NetConnection client){
			sendPkt(pkt, client, false);
		}
		
		private void sendPkt(Packet pkt, NetConnection client, bool secure){
			NetOutgoingMessage outMsg = server.CreateMessage();
			pkt.encode(ref outMsg);
			if(secure)
				server.SendMessage(outMsg, client, NetDeliveryMethod.ReliableOrdered);
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
				server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
			else
				server.SendToAll(outMsg, NetDeliveryMethod.Unreliable);
		}
		
		public bool isRunning(){
			return running;
		}
		
	}
}

