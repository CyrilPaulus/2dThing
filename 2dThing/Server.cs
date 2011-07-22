using System;
using System.Threading;
using _2dThing.GameContent;
using _2dThing.Utils;
using Lidgren.Network;
using System.Collections.Generic;

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
			this.ticker = new Ticker();
			this.map = new World();
			lastTickTime = DateTime.Now;
			NetPeerConfiguration netConfiguration  = new NetPeerConfiguration("2dThing");
			netConfiguration.Port = 55017;
			server = new NetServer(netConfiguration);
			clientList = new List<NetworkClient>();
		}
		
		public void run()
		{
			server.Start();
			Console.WriteLine("Server started");
			while(true)
			{
				if(ticker.Tick())
				{
					update((float) (DateTime.Now - lastTickTime).TotalSeconds);
					lastTickTime = DateTime.Now;
				}
				else
				{
					Thread.Sleep(10);
				}
			}
			
		}
		
				
		public void update(float time){
			readIncomingMsg();			
		}
		
		public void readIncomingMsg(){
			NetIncomingMessage msg;
			while ((msg = server.ReadMessage()) != null)
			{
			    switch (msg.MessageType)
			    {
			        case NetIncomingMessageType.VerboseDebugMessage:
			        case NetIncomingMessageType.DebugMessage:
			        case NetIncomingMessageType.WarningMessage:
				    case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
			            break;
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
						if(status == NetConnectionStatus.Connected){
							int id = getUniqueClientId();
							NetworkClient newClient = new NetworkClient(id, msg.SenderConnection);
							clientList.Add(newClient);
							Console.WriteLine(newClient.Pseudo + " connected");
							
							NetOutgoingMessage outMsg = server.CreateMessage();
							ClientInfo ci = new ClientInfo(id);
							ci.Pseudo = newClient.Pseudo;
							ci.encode(ref outMsg);
							server.SendMessage(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);						
							
						
						}
						else if(status == NetConnectionStatus.Disconnected){
							foreach (NetworkClient c in clientList){
								if(c.Connection == msg.SenderConnection){
									clientList.Remove(c);
									Console.WriteLine("Client " + c.Pseudo + " disconnected");
								break;
								}
							}
						}						
					break;	
					
					case NetIncomingMessageType.Data:
						readPacket(msg);
						break;
			        default:
			            Console.WriteLine("Unhandled type: " + msg.MessageType);
			            break;
			    }
			    server.Recycle(msg);
			}
		}
		
		public void readPacket(NetIncomingMessage msg){
			
			switch(msg.PeekByte()){				
				case Packet.CLIENTINFO:
					foreach(NetworkClient c in clientList){
						if(c.Connection.Equals(msg.SenderConnection)){
							ClientInfo ci = ClientInfo.decode(ref msg);						
							Console.WriteLine("Client " + c.Pseudo + " changed pseudo for " + ci.Pseudo);
							c.Pseudo = ci.Pseudo;
						}
					}
					break;
			case Packet.USERMESSAGE:
				UserMessage uMsg = UserMessage.decode(ref msg);
				//Console.WriteLine(uMsg.Time.Millisecond);
				break;
				default:
					Console.WriteLine("Unsupported packet recieved");
					break;
			}
		}
		
		private int getUniqueClientId(){
			return clientId++;
		}
		
	}
}

