using System;
using System.Threading;
using _2dThing.GameContent;
using _2dThing.Utils;
using Lidgren.Network;
using System.Collections.Generic;
using SFML.Window;

namespace _2dThing {
	public class Server {
		private Ticker ticker;
		private World map;
		private DateTime lastTickTime;
		private NetServer server;
		private Dictionary<NetConnection, NetworkClient> clientList;
		private int clientId = 1;
		private bool running = false;
		private ImageManager imageManager;
		
		public Server(ImageManager imageManager) {
			this.imageManager = imageManager;
			this.ticker = new Ticker();
			
			NetPeerConfiguration netConfiguration = new NetPeerConfiguration("2dThing");			
			netConfiguration.Port = 55017;			
			server = new NetServer(netConfiguration);
			clientList = new Dictionary<NetConnection, NetworkClient>();
		}
		
		private void Reset() {
			this.map = new World(imageManager);
			this.map.AddCube(new Vector2f(0, 90), 0, World.LAYERNBR - 1);
			
			lastTickTime = DateTime.Now;
			clientList.Clear();
		}
		
		public void Run() {
			running = true;	
			
			Reset();	
			
			server.Start();
			Console.WriteLine("Server started");
			
			while (running) {
				if (ticker.Tick()) {
					Update((float)(DateTime.Now - lastTickTime).TotalSeconds);
					lastTickTime = DateTime.Now;
				} else {
					Thread.Sleep(10);
				}
			}			
			
		}
		
		public void Stop() {
			running = false;
			Console.WriteLine("Shutting server down");
			server.Shutdown("Time to sleep bitches");
			while (server.Status != NetPeerStatus.NotRunning) {				
				Thread.Sleep(10);
			}
		}
				
		public void Update(float time) {
			ReadIncomingMsg();			
		}
		
		public void ReadIncomingMsg() {
			NetIncomingMessage msg;
			while ((msg = server.ReadMessage()) != null) {
				switch (msg.MessageType) {
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
						if (status == NetConnectionStatus.Connected) {
							int id = GetUniqueClientId();
							NetworkClient newClient = new NetworkClient(id, msg.SenderConnection);
							clientList.Add(msg.SenderConnection, newClient);
							Console.WriteLine(newClient.Pseudo + " connected");
							
						
							ClientInfo ci = new ClientInfo(id);
							ci.Pseudo = newClient.Pseudo;
							SendPkt(ci, msg.SenderConnection, true);
						
							Player p = new Player(map, imageManager);
							map.AddPlayer(p);
							newClient.Player = p;
							SendFullWorldUpdate(msg.SenderConnection);
							SendFullClientInfo(msg.SenderConnection);
							
						
						} else if (status == NetConnectionStatus.Disconnected) {
							if (clientList.ContainsKey(msg.SenderConnection)) {						
								NetworkClient c = clientList[msg.SenderConnection];
								clientList.Remove(c.Connection);
								map.DeletePlayer(c.Player);
								Console.WriteLine("Client " + c.Pseudo + " disconnected");
								
								ClientDisconnect cd = new ClientDisconnect(c.ClientId);
								SendPktToAll(cd, true);								
							}
												
						}						
						break;	
					
					case NetIncomingMessageType.Data:
						ReadPacket(msg);
						break;
					
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
					
				}
				server.Recycle(msg);
			}
		}
		
		public void ReadPacket(NetIncomingMessage msg) {
			
			switch (msg.PeekByte()) {				
				case Packet.CLIENTINFO:				
					if (clientList.ContainsKey(msg.SenderConnection)) {
						NetworkClient c = clientList[msg.SenderConnection];
						ClientInfo ci = ClientInfo.decode(ref msg);						
						Console.WriteLine("Client " + c.Pseudo + " changed pseudo for " + ci.Pseudo);
						c.Pseudo = ci.Pseudo;
						c.Player.Color = ci.Color;
						SendPktToAll(ci);
					}				
					break;
			
				case Packet.USERMESSAGE:
					UserMessage uMsg = UserMessage.decode(ref msg);
					if (clientList.ContainsKey(msg.SenderConnection)) {
						NetworkClient c = clientList[msg.SenderConnection];
						c.Player.Layer = uMsg.Layer;
						c.Player.Noclip = uMsg.Nolcip;
						c.Player.Update((float)(uMsg.Time - c.LastUpdate).TotalSeconds, uMsg.Input);
						c.LastUpdate = uMsg.Time;	
						uMsg.Position = c.Player.Position;
						SendPktToAll(uMsg, true);				
					}
					break;
			
				case Packet.BLOCKUPDATE:				
					BlockUpdate bu = BlockUpdate.decode(ref msg);
				
					if ((bu.Added && map.AddCube(bu.Position, bu.BlockType, bu.Layer)) || !bu.Added) {
						SendPktToAll(bu);
					}
					
					if (!bu.Added)
						map.DeleteCube(bu.Position, bu.Layer);		
				
					break;
			
				case Packet.CLIENTRESET:
					if (clientList.ContainsKey(msg.SenderConnection)) {
						NetworkClient c = clientList[msg.SenderConnection];
						c.Player.Reset();
					}
					break;
				case Packet.CHATMESSAGE:
					ChatMessage cm = ChatMessage.decode(ref msg);
					SendPktToAll(cm, true);
					Console.WriteLine(cm.Pseudo + ": " + cm.Message);
					break;
			
				default:
					Console.WriteLine("Unsupported packet recieved");
					break;
			}
		}
		
		private int GetUniqueClientId() {
			return clientId++;
		}
		
		private void SendFullWorldUpdate(NetConnection client) {
			int layer = 0;
			foreach (List<Cube> cubeList in map.CubeLists) {
				foreach (Cube c in cubeList) {
					BlockUpdate bu = new BlockUpdate(0);
					bu.Added = true;
					bu.Position = c.Position;
					bu.BlockType = (byte)c.type;
					bu.Layer = (byte)layer;
					SendPkt(bu, client, true);
				}
				layer++;
			}
		}
		
		private void SendFullClientInfo(NetConnection client) {
			foreach (NetworkClient c in clientList.Values) {
				ClientInfo ci = new ClientInfo(c.ClientId);
				ci.Color = c.Player.Color;
				ci.Pseudo = c.Pseudo;
				SendPkt(ci, client, true);
			}
		}
		
		private void SendPkt(Packet pkt, NetConnection client) {
			SendPkt(pkt, client, false);
		}
		
		private void SendPkt(Packet pkt, NetConnection client, bool secure) {
			NetOutgoingMessage outMsg = server.CreateMessage();
			pkt.encode(ref outMsg);
			if (secure)
				server.SendMessage(outMsg, client, NetDeliveryMethod.ReliableOrdered);
			else
				server.SendMessage(outMsg, client, NetDeliveryMethod.Unreliable);
		}
		
		private void SendPktToAll(Packet pkt) {
			SendPktToAll(pkt, false);
		}
		
		private void SendPktToAll(Packet pkt, bool secure) {
			NetOutgoingMessage outMsg = server.CreateMessage();
			pkt.encode(ref outMsg);
			if (secure)
				server.SendToAll(outMsg, NetDeliveryMethod.ReliableOrdered);
			else
				server.SendToAll(outMsg, NetDeliveryMethod.Unreliable);
		}
		
		public bool IsRunning() {
			return running;
		}
		
		public void SaveMap(String filename){		
				map.SaveMap(filename);			
		}
	}
}

