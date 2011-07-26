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

namespace _2dThing
{
	public class Client
	{
		RenderWindow window;
		RenderImage world;
		RenderImage ui;
		
		int blockType = 0;
		int clientId = 0;
		String pseudo = "Anon";
		bool hasId = false;
		
		Ticker ticker;
		
		Sprite mouse;		
		World map;
		InputManager inputManager;
		
		NetClient client;
		Dictionary<int, NetworkClient> otherClients;
		String ip = "localhost";
		
		
		Font myFont;		
		
		Chat chat;
		Cube blockTypeDisplay;
		
		   
		Player player;
		UserMessageBuffer uMsgBuffer;

		public Client ()
		{
			window = new RenderWindow (new VideoMode (800, 600), "2dThing is back bitches");
			world = new RenderImage (800, 600);
			ui = new RenderImage (800, 600);
						
			map = new World ();
			ticker = new Ticker ();            

			window.Closed += new EventHandler (OnClose);
			window.Resized += new EventHandler<SizeEventArgs>(OnWindowResized);	
			
			window.ShowMouseCursor (false);
			window.SetFramerateLimit (60);
			
			mouse = new Sprite (new Image ("content/mouse.png"));

			player = new Player (map);			
			map.addPlayer (player);
			world.DefaultView.Center = new Vector2f (0, 0);
			world.SetView (world.DefaultView);
			
			NetPeerConfiguration netConfiguration = new NetPeerConfiguration ("2dThing");			
			client = new NetClient (netConfiguration);
			uMsgBuffer = new UserMessageBuffer();
			otherClients = new Dictionary<int, NetworkClient>();
			
			chat = new Chat(this);
			inputManager = new InputManager(this);
			myFont = new Font ("content/arial.ttf");
			blockTypeDisplay = new Cube(blockType);
			blockTypeDisplay.Position = new Vector2f(ui.Width - 2*Cube.WIDTH, ui.Height - 2* Cube.HEIGHT);
		}
		
		public Client(String ip) : this()
		{
			this.ip = ip;
		}

		public void run ()
		{
			client.Start ();			
					
			client.Connect (ip, 55017);			
			
			while (client.ConnectionStatus == NetConnectionStatus.Disconnected) {
				Thread.Sleep (10);
			}
			
						
			Console.WriteLine ("Client connected");
			
			while (!hasId) {
				readIncomingMsg ();
				Thread.Sleep (10);
			}
			
			
			NetOutgoingMessage msg = client.CreateMessage ();
			ClientInfo ci = new ClientInfo (clientId);
			ci.Color = player.Color;
			ci.Pseudo = pseudo;
			sendPkt(ci, true);
				
			         
            
			DateTime lastTickTime = DateTime.Now;
			
			Text uiText = new Text ("Tps:", myFont);
			uiText.Position = new Vector2f (0, 0);
			uiText.CharacterSize = 14;
			
			
			
			
			while (window.IsOpened()) {
				
				window.DispatchEvents ();

				if (ticker.Tick ()) {
					float ticktime = (float)(DateTime.Now - lastTickTime).TotalSeconds;
					readIncomingMsg ();
					update (ticktime);
					uiText.DisplayedString = "Fps: " + (int)(1f / window.GetFrameTime () * 1000) + "\nTps: " + (int)(1 / (DateTime.Now - lastTickTime).TotalSeconds) + "\nClientId: " + clientId;
					lastTickTime = DateTime.Now;					
				}
				

				world.Clear (new Color (100, 149, 237));
				map.Draw (world);
				drawPlayersPseudo();				
				world.Display ();						

				ui.Clear (new Color (255, 255, 255, 0));
				ui.Draw (mouse);				
				ui.Draw (uiText);
				blockTypeDisplay.Draw(ui);
				chat.Draw(ui);
				ui.Display ();

				window.Clear (new Color (100, 149, 237));                
				window.Draw (new Sprite (world.Image));
				window.Draw (new Sprite (ui.Image));				
				window.Display ();                
			}
			
			Console.WriteLine ("Disconnecting");			
			client.Disconnect ("Bye bitches");
			while (client.ConnectionStatus != NetConnectionStatus.Disconnected) {
				Thread.Sleep (10);
			}
			
			
		}

		/// <summary>
		/// Update the game state
		/// </summary>
		/// <param name="frameTime">time of last frame in seconds</param>
		private void update (float frameTime)
		{
			
			if (inputManager.Input.LeftMouseButton){				
				BlockUpdate bu = new BlockUpdate(clientId);
				bu.Added = true;
				bu.Position = getWorldMouse();
				bu.BlockType = (byte) blockType;
				sendPkt(bu);				
			}
			
			if (inputManager.Input.RightMouseButton){				
				BlockUpdate bu = new BlockUpdate(clientId);
				bu.Added = false;
				bu.Position = getWorldMouse();
				sendPkt(bu);
			}
			
			player.lookAt (getWorldMouse ());
			player.update (frameTime, inputManager.Input);
			updateCam ();
			
			UserMessage uMsg = new UserMessage (clientId);
			uMsg.Position = player.Position;
			uMsg.Ticktime = frameTime;
			uMsg.EyePosition = getWorldMouse();
			uMsg.FallSpeed = player.FallSpeed;				
			uMsg.Input = inputManager.Input;			
			sendPkt(uMsg);
			
			uMsgBuffer.insert(uMsg);
		}

		//Events
		static void OnClose (object sender, EventArgs e)
		{
			RenderWindow window = (RenderWindow)sender;
			window.Close ();
		}

			
		void OnWindowResized(object sender, EventArgs e){
			SizeEventArgs a = (SizeEventArgs) e;			
			
			View newView = new View(new FloatRect(0,0,a.Width, a.Height));
			window.SetView(newView);
			
			world = new RenderImage(a.Width, a.Height);
			world.DefaultView.Center = player.Position;
			ui = new RenderImage(a.Width, a.Height);
			
			
		}
		
		public Vector2f getWorldMouse ()
		{
			return world.ConvertCoords ((uint)mouse.Position.X, (uint)mouse.Position.Y);
		}

		public void updateCam ()
		{
            
           
			float left = world.DefaultView.Center.X - world.DefaultView.Size.X / 2;
			float right = world.DefaultView.Center.X + world.DefaultView.Size.X / 2;
			if (player.Bbox.Left - 100 < left)
				world.DefaultView.Move (new Vector2f (player.Bbox.Left - 100 - left, 0));
			else if (player.Bbox.Left + player.Bbox.Width + 100 > right)
				world.DefaultView.Move (new Vector2f (player.Bbox.Left + player.Bbox.Width + 100 - right, 0));


			float top = world.DefaultView.Center.Y - world.DefaultView.Size.Y / 2;
			float bottom = world.DefaultView.Center.Y + world.DefaultView.Size.Y / 2;

			if (player.Bbox.Top - 100 < top)
				world.DefaultView.Move (new Vector2f (0, player.Bbox.Top - 100 - top));
			else if (player.Bbox.Top + player.Bbox.Height + 100 > bottom)
				world.DefaultView.Move (new Vector2f (0, player.Bbox.Top + player.Bbox.Height + 100 - bottom));

			world.SetView (world.DefaultView);
		}
		
		public void readIncomingMsg ()
		{
			NetIncomingMessage msg;
			while ((msg = client.ReadMessage()) != null) {
				switch (msg.MessageType) {
				case NetIncomingMessageType.VerboseDebugMessage:
				case NetIncomingMessageType.DebugMessage:
				case NetIncomingMessageType.WarningMessage:
				case NetIncomingMessageType.ErrorMessage:
					Console.WriteLine (msg.ReadString ());
					break;					
				case NetIncomingMessageType.Data:
					readPacket (msg);
					break;
				default:
					Console.WriteLine ("Unhandled type: " + msg.MessageType);
					break;
				}
				client.Recycle (msg);
			}
		}
		
		public void readPacket (NetIncomingMessage msg)
		{
			switch (msg.PeekByte ()) {
				
			case Packet.CLIENTINFO:
				ClientInfo ci = ClientInfo.decode (ref msg);
				if(!hasId){				
					clientId = ci.ClientId;
					hasId = true;
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
						newClient.Player = new Player(map);
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
					map.forceAddCube(bu.Position, bu.BlockType);
				else
					map.deleteCube(bu.Position);
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
		
		
		public void sendPkt(Packet pkt){
			sendPkt(pkt, false);
		}
		
		public void sendPkt(Packet pkt, bool secure){
			NetOutgoingMessage outMsg = client.CreateMessage();
			pkt.encode(ref outMsg);
			if(secure)
				client.SendMessage(outMsg, NetDeliveryMethod.ReliableUnordered);
			else
				client.SendMessage(outMsg, NetDeliveryMethod.Unreliable);
		}
		
		private void drawPlayersPseudo(){
			
			foreach (NetworkClient c in otherClients.Values){				
				Text pseudo = new Text(c.Pseudo, myFont);
				pseudo.CharacterSize = 12;
				pseudo.Color = Color.White;
				pseudo.Position = c.Player.Position - new Vector2f(pseudo.GetRect().Width / 2 - c.Player.Bbox.Width / 2, 20);
				world.Draw(pseudo);
			}
		}
		
		public string Pseudo{
			get {return pseudo;}
			set {pseudo = value;}
		}
		
		public int ClientId{
			get {return clientId;}
		}
		
		public RenderWindow MainWindow{
			get {return window;}			
		}
		
		public Sprite Mouse{
			get {return mouse;}
		}
		
		public Chat Chat{
			get {return chat;}
		}
		
		public Player Player{
			get {return player;}
		}
		
		public void Zoom(float value){
			world.DefaultView.Zoom(value);
		}
		
		public int BlockType{
			get { return blockType;}
			set { blockType = value; blockTypeDisplay.setType(value); }
		}
	}
}
