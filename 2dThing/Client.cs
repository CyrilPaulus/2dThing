using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using _2dThing.Utils;
using _2dThing.GameContent;
using Lidgren.Network;

namespace _2dThing {
	public class Client : Screen {
		
		private RenderImage world;	
		
		private int clientId = -1;
		private String pseudo = "Anon";		
		
		private Ticker ticker;
		
		private Sprite mouse;		
		private World map;
		private InputManager inputManager;		
		
		private NetClient client;
		private Dictionary<int, NetworkClient> otherClients;
		private String ip = "localhost";
		
		private float zoom = 1;
		private Font myFont;		
		
		private Chat chat;
		
		private int blockType = 0;
		private Cube blockTypeDisplay;
		
		   
		private Player player;
		private UserMessageBuffer uMsgBuffer;
		private LayerDisplay layerDisplay;		

		public Client (RenderWindow window, ImageManager imageManager) : base(window, imageManager) {
			this.window = window;
			world = new RenderImage(800, 600);			
			
			inputManager = new InputManager(this);						
			
			ticker = new Ticker(); 
					
			window.ShowMouseCursor (false);
			window.SetFramerateLimit (60);
			
			NetPeerConfiguration netConfiguration = new NetPeerConfiguration("2dThing");			
			client = new NetClient(netConfiguration);
			
			uMsgBuffer = new UserMessageBuffer();
			otherClients = new Dictionary<int, NetworkClient>();			
			chat = new Chat(this);
			
			LoadRessources();
			
			blockTypeDisplay = new Cube(blockType, imageManager);
			blockTypeDisplay.Position = new Vector2f(window.Width - 2*Cube.WIDTH, window.Height - 2* Cube.HEIGHT);
			layerDisplay = new LayerDisplay(imageManager);
			layerDisplay.Position = blockTypeDisplay.Position - new Vector2f(0, 50);			
			
			mouse = new Sprite (imageManager.GetImage("mouse"));						
		}
		
		public void Connect(){			
			Reset();
			
			client.Start();				
			client.Connect (ip, 55017);			
			
			DateTime start = DateTime.Now;
			while (client.ConnectionStatus == NetConnectionStatus.Disconnected) {
				Thread.Sleep (10);
				if((DateTime.Now - start).TotalSeconds > 20)
					return;
			}			
						
			Console.WriteLine ("Connecting... Waiting for ID");
			
			while (clientId == -1) {
				ReadIncomingMsg ();
				Thread.Sleep (10);
			}			
			
			Console.WriteLine ("Client connected");
			
			NetOutgoingMessage msg = client.CreateMessage ();
			ClientInfo ci = new ClientInfo (clientId);
			ci.Color = player.Color;
			ci.Pseudo = pseudo;
			SendPacket(ci, true);			
		}
		
		public void Disconnect() {
			Console.WriteLine ("Disconnecting");
			client.Disconnect ("Bye");
			while (client.ConnectionStatus != NetConnectionStatus.Disconnected) {
				Thread.Sleep (10);
			}
		}		
		
		public override int Run() {					
			//Resize the window if size changed in another screen        
           	Resize(window.Width, window.Height);
			
			DateTime lastTickTime = DateTime.Now;			
						
			while (window.IsOpened()) {			
				window.DispatchEvents ();				
			
				if(inputManager.MainMenu || client.ConnectionStatus == NetConnectionStatus.Disconnected) {
					inputManager.MainMenu = false;					
					return Screen.MAINMENU;
				}
							
				if (ticker.Tick()) {
					float ticktime = (float)(DateTime.Now - lastTickTime).TotalSeconds;
					ReadIncomingMsg();
					Update(ticktime);					
					lastTickTime = DateTime.Now;					
				}				
				
				Draw();				                
			}

			Disconnect();			
			return -1;			
		}
		
		private void Reset() {
			otherClients.Clear();
			clientId = -1;						
			
			map = new World (imageManager);
			player = new Player (map, imageManager);			
			map.addPlayer (player);
			world.DefaultView.Center = new Vector2f (0, 0);
			world.SetView (world.DefaultView);			
		}
		
		private void Draw() {
			world.Clear (new Color (100, 149, 237));			
			map.Draw(world, player.Layer);	
			
			if(inputManager.Input.UpperLayer)
				map.DrawUpperLayer(world, player.Layer);
				
			DrawPlayersPseudo();			
			world.Display ();						
				
			window.Clear (new Color (100, 149, 237)); 			
			window.Draw (new Sprite (world.Image));
			window.Draw (mouse);			
			layerDisplay.Draw(window, player.Layer);
			blockTypeDisplay.Draw(window);
			chat.Draw(window);				
			window.Display();
		}
		
		private void Update(float frameTime) {			
			if (inputManager.Input.LeftMouseButton) {				
				BlockUpdate bu = new BlockUpdate(clientId);
				bu.Added = true;
				bu.Position = GetWorldMouse();
				bu.BlockType = (byte) blockType;
				bu.Layer = (byte) player.Layer;
				SendPacket(bu);				
			}
			
			if (inputManager.Input.RightMouseButton) {				
				BlockUpdate bu = new BlockUpdate(clientId);
				bu.Added = false;
				bu.Position = GetWorldMouse();
				bu.Layer = (byte) player.Layer;
				SendPacket(bu);
			}
			
			player.lookAt(GetWorldMouse());
			player.update(frameTime, inputManager.Input);
			UpdateCam();
			
			UserMessage uMsg = new UserMessage(clientId);
			uMsg.Position = player.Position;
			uMsg.Ticktime = frameTime;
			uMsg.EyePosition = GetWorldMouse();
			uMsg.FallSpeed = player.FallSpeed;				
			uMsg.Input = inputManager.Input;
			uMsg.Layer = (byte) player.Layer;
			uMsg.Nolcip = player.Noclip;
			SendPacket(uMsg);
			
			uMsgBuffer.insert(uMsg);
		}

//EVENTS MANAGEMENT
		
		private void OnClose(object sender, EventArgs e) {
			RenderWindow window = (RenderWindow)sender;
			window.Close ();
		}
			
		private void OnWindowResized(object sender, EventArgs e) {
			SizeEventArgs a = (SizeEventArgs) e;			
			Resize(a.Width, a.Height);		
		}
		
		public override void loadEventHandler() {			
			window.Closed += new EventHandler (OnClose);
			window.Resized += new EventHandler<SizeEventArgs>(OnWindowResized);	
			inputManager.loadEventHandler();
		}
		
		public override void unloadEventHandler() {
			window.Closed -= new EventHandler (OnClose);
			window.Resized -= new EventHandler<SizeEventArgs>(OnWindowResized);	
			inputManager.unloadEventHandler();		
		}
		
//UTILS	
		
		private void DrawPlayersPseudo() {			
			foreach (NetworkClient c in otherClients.Values){				
				Text pseudo = new Text(c.Pseudo, myFont);
				pseudo.CharacterSize = 12;
				pseudo.Color = Color.White;
				pseudo.Position = c.Player.Position - new Vector2f(pseudo.GetRect().Width / 2 - c.Player.Bbox.Width / 2, 20);
				world.Draw(pseudo);
			}
		}
		
		private Vector2f GetWorldMouse() {
			return world.ConvertCoords ((uint)mouse.Position.X, (uint)mouse.Position.Y);
		}
		
		private void LoadRessources() {						
			myFont = new Font ("content/arial.ttf");
		}
		
		private void Resize(uint width, uint height) {
			View newView = new View(new FloatRect(0,0,width, height));
			window.SetView(newView);
			
			world = new RenderImage(width, height);
			world.DefaultView.Center = player.Center;
			world.DefaultView.Zoom(zoom);
			
			blockTypeDisplay.Position = new Vector2f(window.Width - 2*Cube.WIDTH, window.Height - 2* Cube.HEIGHT);
			layerDisplay.Position = blockTypeDisplay.Position - new Vector2f(0, 50);
		}
		
		private void UpdateCam () {
            float left = world.DefaultView.Center.X - world.DefaultView.Size.X / 2;
			float right = world.DefaultView.Center.X + world.DefaultView.Size.X / 2;
			if (player.Bbox.Left - 100 * zoom < left)
				world.DefaultView.Move (new Vector2f (player.Bbox.Left - 100 * zoom - left, 0));
			else if (player.Bbox.Left + player.Bbox.Width + 100 * zoom > right)
				world.DefaultView.Move (new Vector2f (player.Bbox.Left + player.Bbox.Width + 100 * zoom - right, 0));

			float top = world.DefaultView.Center.Y - world.DefaultView.Size.Y / 2;
			float bottom = world.DefaultView.Center.Y + world.DefaultView.Size.Y / 2;
			if (player.Bbox.Top - 100 * zoom < top)
				world.DefaultView.Move (new Vector2f (0, player.Bbox.Top - 100 * zoom - top));
			else if (player.Bbox.Top + player.Bbox.Height + 100 * zoom > bottom)
				world.DefaultView.Move (new Vector2f (0, player.Bbox.Top + player.Bbox.Height + 100 * zoom - bottom));

			world.SetView (world.DefaultView);
		}		
		
		public void Zoom(float value) {
			world.DefaultView.Zoom(value);
			world.DefaultView.Center = player.Center;
			zoom *= value;
		}
		
//Network
		
		private void ReadIncomingMsg() {
			NetIncomingMessage msg;
			while ((msg = client.ReadMessage()) != null) {
				switch (msg.MessageType) {
				case NetIncomingMessageType.VerboseDebugMessage:
				case NetIncomingMessageType.DebugMessage:
				case NetIncomingMessageType.WarningMessage:
				case NetIncomingMessageType.ErrorMessage:
					Console.WriteLine(msg.ReadString ());
					break;					
				case NetIncomingMessageType.Data:
					ReadPacket(msg);
					break;
				default:
					Console.WriteLine("Unhandled type: " + msg.MessageType);
					break;
				}
				client.Recycle(msg);
			}
		}
		
		private void ReadPacket(NetIncomingMessage msg) {			
			switch (msg.PeekByte ()) {
			case Packet.CLIENTINFO:
				ClientInfo ci = ClientInfo.decode (ref msg);
				if(clientId == - 1){				
					clientId = ci.ClientId;
				}
				else if (ci.ClientId != clientId ){				
				
					if(otherClients.ContainsKey(ci.ClientId)){
						NetworkClient c = otherClients[ci.ClientId];
						c.Pseudo = ci.Pseudo;
						c.Player.Color = ci.Color;						
					}
					else
					{
						NetworkClient newClient = new NetworkClient(ci.ClientId, null);
						newClient.Player = new Player(map, imageManager);
						newClient.Player.Color = ci.Color;
						newClient.Pseudo = ci.Pseudo;
						map.addPlayer(newClient.Player);
						otherClients.Add(newClient.ClientId, newClient);
					}
				}
				break;
			
			case Packet.USERMESSAGE:
				UserMessage uMsg = UserMessage.decode(ref msg);
				if(uMsg.ClientId == clientId){
					uMsgBuffer.clientCorrection(player, uMsg);					
				}
				else
				{					
					if(otherClients.ContainsKey(uMsg.ClientId)){
						NetworkClient c = otherClients[uMsg.ClientId];
						c.Player.lookAt(uMsg.EyePosition);
						c.Player.Layer = uMsg.Layer;
						if(VectorUtils.Distance(c.Player.Position, uMsg.Position) > 1){
							c.Player.Position = uMsg.Position;								
						}
						else
							c.Player.Position += (c.Player.Position - uMsg.Position) * 0.1F;							
						return;
					}
				}								
				break;
			
			case Packet.BLOCKUPDATE:				
				BlockUpdate bu = BlockUpdate.decode(ref msg);
				if(bu.Added)
					map.forceAddCube(bu.Position, bu.BlockType, bu.Layer);
				else
					map.deleteCube(bu.Position, bu.Layer);
				break;
			
			case Packet.CLIENTDISCONNECT:
				ClientDisconnect cd = ClientDisconnect.decode(ref msg);				
				
				if(otherClients.ContainsKey(cd.ClientId)){
					NetworkClient c = otherClients[cd.ClientId];
					otherClients.Remove(c.ClientId);
					map.deletePlayer(c.Player);				
				}
				break;
				
			case Packet.CHATMESSAGE:
				ChatMessage cm = ChatMessage.decode(ref msg);
				chat.addMsg(cm.Pseudo +": " + cm.Message);
				break;
			
			default:
				break;
			}
		}		
		
		public void SendPacket(Packet pkt) {
			SendPacket(pkt, false);
		}
		
		public void SendPacket(Packet pkt, bool secure) {
			NetOutgoingMessage outMsg = client.CreateMessage();
			pkt.encode(ref outMsg);
			if(secure)
				client.SendMessage(outMsg, NetDeliveryMethod.ReliableUnordered);
			else
				client.SendMessage(outMsg, NetDeliveryMethod.Unreliable);
		}
		
//Accessor Mutator	
		
		public int BlockType {
			get { return blockType; }
			set { blockType = value; blockTypeDisplay.setType(value); }
		}
		
		public Chat Chat {
			get { return chat; }
		}
		
		public int ClientId {
			get { return clientId; }
		}
		
		public String IP {
			get { return ip; }
			set { ip = value; }
		}
		
		public RenderWindow MainWindow {
			get { return window; }			
		}
		
		public Sprite Mouse {
			get { return mouse; }
		}		
		
		public Player Player {
			get { return player; }
		}
		
		public string Pseudo {
			get { return pseudo; }
			set { pseudo = value; }
		}		
		
		public bool isRunning() {
			return client.Status == NetPeerStatus.Running;
		}		
	}
}
