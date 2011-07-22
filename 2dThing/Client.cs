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
	class Client
	{
		RenderWindow window;
		RenderImage world;
		RenderImage ui;
		int clientId = 0;
		bool hasId = false;
		Ticker ticker;
		Sprite mouse;
		World map;
		NetClient client;
		List<NetworkClient> otherClients;

		//TODO Dumb stuff to delete      
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
			window.MouseMoved += new EventHandler<MouseMoveEventArgs> (OnMouseMoved);
			window.KeyPressed += new EventHandler<KeyEventArgs> (OnKeyPressed);
			window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs> (OnMouseButtonPressed);
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
			otherClients = new List<NetworkClient>();
		}

		public void run ()
		{
			client.Start ();			
					
			client.Connect ("localhost", 55017);			
			
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
			ci.Pseudo = "test";
			ci.encode (ref msg);	
			client.SendMessage (msg, NetDeliveryMethod.ReliableUnordered);
				
			//Dumb stuff to remove
			Font myFont = new Font ("content/arial.ttf");            
            
			DateTime lastTickTime = DateTime.Now;
			
			Text uiText = new Text ("Tps:", myFont);
			uiText.Position = new Vector2f (0, 0);
			uiText.CharacterSize = 14;
			//uiText.Color = Color.Black;
			
			
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
				world.Display ();

				ui.Clear (new Color (255, 255, 255, 0));
				ui.Draw (mouse);                
				ui.Draw (uiText);
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
			NetOutgoingMessage msg;
			if (Mouse.IsButtonPressed (Mouse.Button.Left)){				
				BlockUpdate bu = new BlockUpdate(clientId);
				bu.Added = true;
				bu.Position = getWorldMouse();
				msg = client.CreateMessage();
				bu.encode(ref msg);
				client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);				
			}
			
			if (Mouse.IsButtonPressed (Mouse.Button.Right)){				
				BlockUpdate bu = new BlockUpdate(clientId);
				bu.Added = false;
				bu.Position = getWorldMouse();
				msg = client.CreateMessage();
				bu.encode(ref msg);
				client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
			}
			
			player.lookAt (getWorldMouse ());
			Input input = new Input();
			input.Left = Keyboard.IsKeyPressed(Keyboard.Key.Left);
			input.Right = Keyboard.IsKeyPressed(Keyboard.Key.Right);
			input.Up = Keyboard.IsKeyPressed(Keyboard.Key.Up);
			input.Down = Keyboard.IsKeyPressed(Keyboard.Key.Down);
			player.update (frameTime, input);
			updateCam ();
			
			UserMessage uMsg = new UserMessage (clientId);
			uMsg.Position = player.Position;
			uMsg.Ticktime = frameTime;
				
			uMsg.Input = input;
			msg = client.CreateMessage ();
			uMsg.encode (ref msg);
			uMsgBuffer.insert(uMsg);
			client.SendMessage (msg, NetDeliveryMethod.Unreliable);
		}

		//Events
		static void OnClose (object sender, EventArgs e)
		{
			RenderWindow window = (RenderWindow)sender;
			window.Close ();
		}

		void OnMouseMoved (object sender, EventArgs e)
		{
			MouseMoveEventArgs a = (MouseMoveEventArgs)e;
			mouse.Position = new Vector2f (a.X, a.Y);            
		}

		void OnMouseButtonPressed (object sender, EventArgs e)
		{
			MouseButtonEventArgs a = (MouseButtonEventArgs)e;          
		}

		void OnKeyPressed (object sender, EventArgs e)
		{
			KeyEventArgs a = (KeyEventArgs)e;
			switch (a.Code) {
			case Keyboard.Key.Left:
                    //world.DefaultView.Move(new Vector2f(-10, 0));                    
				world.SetView (world.DefaultView);                   
				break;
			case Keyboard.Key.Right:
                    //world.DefaultView.Move(new Vector2f(10, 0));
				world.SetView (world.DefaultView);
				break;
			case Keyboard.Key.Up:
                    //world.DefaultView.Move(new Vector2f(0, -10));
				world.SetView (world.DefaultView);
				break;
			case Keyboard.Key.Down:
                    //world.DefaultView.Move(new Vector2f(0, 10));
				world.SetView (world.DefaultView);
				break;
			case Keyboard.Key.R:
				player.Position = new Vector2f (0, 0);
				break;
			default:
				break;
			}
		}

		private Vector2f getWorldMouse ()
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
				clientId = ci.ClientId;
				hasId = true;
				break;
			case Packet.USERMESSAGE:
				UserMessage uMsg = UserMessage.decode(ref msg);
				if(uMsg.ClientId == clientId){
					uMsgBuffer.clientCorrection(player, uMsg);					
				}else{
					foreach(NetworkClient c in otherClients){
						if(c.ClientId == uMsg.ClientId){
							if(VectorUtils.Distance(c.Player.Position, uMsg.Position) > 2){
								c.Player.Position = uMsg.Position;
							}else
								c.Player.Position += (c.Player.Position - uMsg.Position) * 0.1F;							
							return;
						}
					}
					
					NetworkClient newClient = new NetworkClient(uMsg.ClientId, null);
					newClient.Player = new Player(map);
					map.addPlayer(newClient.Player);
					otherClients.Add(newClient);
				}
				break;
			case Packet.BLOCKUPDATE:
				BlockUpdate bu = BlockUpdate.decode(ref msg);
				if(bu.Added)
					map.forceAddCube(bu.Position);
				else
					map.deleteCube(bu.Position);
				break;
			default:
				break;
			}
		}
	}
}
