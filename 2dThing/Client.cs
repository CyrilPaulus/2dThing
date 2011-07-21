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

        Ticker ticker;

        Sprite mouse;

        World map;
		
		NetClient client;		

        //TODO Dumb stuff to delete      
        Player player;

        public Client()
        {
            window = new RenderWindow(new VideoMode(800, 600), "2dThing is back bitches");
            world = new RenderImage(800, 600);
            ui = new RenderImage(800, 600);
            map = new World();
            ticker = new Ticker();
            //ticker.TPS = 60;

            window.Closed += new EventHandler(OnClose);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);
            window.ShowMouseCursor(false);
            window.SetFramerateLimit(60);
            mouse = new Sprite(new Image("content/mouse.png"));

            player = new Player(map);
            map.addCube(new Vector2f(0,80));
            map.addPlayer(player);
            world.DefaultView.Center = new Vector2f(0, 0);
            world.SetView(world.DefaultView);
			
			NetPeerConfiguration netConfiguration = new NetPeerConfiguration("2dThing");			
			client = new NetClient(netConfiguration);
        }

        public void run()
        {
			client.Start();			
					
			client.Connect("localhost", 55017);			
			
			while(client.ConnectionStatus == NetConnectionStatus.Disconnected){
				Thread.Sleep(10);
			}
			
			Console.WriteLine("Client connected");
			
			NetOutgoingMessage msg = client.CreateMessage();
			ClientInfo ci = new ClientInfo();
			ci.Pseudo = "test";
			ci.encode(ref msg);	
			client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
				
            //Dumb stuff to remove
            Font myFont = new Font("content/arial.ttf");            
            Text fps = new Text("Fps:", myFont);
            fps.Position = new Vector2f(0, 0);
            fps.CharacterSize = 20;
            fps.Color = Color.Black;
            DateTime lastTickTime = DateTime.Now;
			
			Text tps = new Text("Tps:", myFont);
			tps.Position = new Vector2f(0, 20);
			tps.CharacterSize = 20;
			tps.Color = Color.Black;
			
			
            while (window.IsOpened())
            {
                if (window.GetFrameTime() != 0)
                {
                    fps.DisplayedString = "Fps: " + (int)(1f / window.GetFrameTime() * 1000);
                }
                window.DispatchEvents();

                if (ticker.Tick())
                {
					readIncomingMsg();
                    update((float) (DateTime.Now - lastTickTime).TotalSeconds);
					tps.DisplayedString = "Tps: " + (int) (1 / (DateTime.Now - lastTickTime).TotalSeconds);
                    lastTickTime = DateTime.Now;					
                }

                world.Clear(new Color(100, 149, 237));
                map.Draw(world);                
                world.Display();

                ui.Clear(new Color(255, 255, 255, 0));
                ui.Draw(mouse);
                ui.Draw(fps);
				ui.Draw(tps);
                ui.Display();

                window.Clear(new Color(100, 149, 237));                
                window.Draw(new Sprite(world.Image));
                window.Draw(new Sprite(ui.Image));
                window.Display();                
            }
			
			Console.WriteLine("Disconnecting");			
			client.Disconnect("Bye bitches");
			while(client.ConnectionStatus != NetConnectionStatus.Disconnected){
				Thread.Sleep(10);
			}
			
			
        }

        /// <summary>
        /// Update the game state
        /// </summary>
        /// <param name="frameTime">time of last frame in seconds</param>
        private void update(float frameTime)
        {
			if(Mouse.IsButtonPressed(Mouse.Button.Left))
				map.addCube(getWorldMouse());
			
			if(Mouse.IsButtonPressed(Mouse.Button.Right))
				map.deleteCube(getWorldMouse());
			
            player.Left = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            player.Right = Keyboard.IsKeyPressed(Keyboard.Key.Right);
            player.Up = Keyboard.IsKeyPressed(Keyboard.Key.Up);
            player.Down = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            player.lookAt(getWorldMouse());
            player.update(frameTime);
            updateCam();
        }

        //Events
        static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        void OnMouseMoved(object sender, EventArgs e)
        {
            MouseMoveEventArgs a = (MouseMoveEventArgs)e;
            mouse.Position = new Vector2f(a.X, a.Y);            
        }

        void OnMouseButtonPressed(object sender, EventArgs e)
        {
            MouseButtonEventArgs a = (MouseButtonEventArgs)e;          
        }

        void OnKeyPressed(object sender, EventArgs e)
        {
            KeyEventArgs a = (KeyEventArgs)e;
            switch (a.Code)
            {
                case Keyboard.Key.Left:
                    //world.DefaultView.Move(new Vector2f(-10, 0));                    
                    world.SetView(world.DefaultView);                   
                    break;
                case Keyboard.Key.Right:
                    //world.DefaultView.Move(new Vector2f(10, 0));
                    world.SetView(world.DefaultView);
                    break;
                case Keyboard.Key.Up:
                    //world.DefaultView.Move(new Vector2f(0, -10));
                    world.SetView(world.DefaultView);
                    break;
                case Keyboard.Key.Down:
                    //world.DefaultView.Move(new Vector2f(0, 10));
                    world.SetView(world.DefaultView);
                    break;
                case Keyboard.Key.R:
                    player.Position = new Vector2f(0, 0);
                    break;
                default:
                    break;
            }
        }

        private Vector2f getWorldMouse()
        {
            return world.ConvertCoords((uint)mouse.Position.X, (uint)mouse.Position.Y);
        }

        public void updateCam()
        {
            
           
            float left = world.DefaultView.Center.X - world.DefaultView.Size.X / 2;
            float right = world.DefaultView.Center.X + world.DefaultView.Size.X / 2;
            if (player.Bbox.Left - 100 < left)
                world.DefaultView.Move(new Vector2f(player.Bbox.Left - 100 - left, 0));
            else if (player.Bbox.Left + player.Bbox.Width + 100 > right)
                world.DefaultView.Move(new Vector2f(player.Bbox.Left + player.Bbox.Width + 100 - right, 0));


            float top = world.DefaultView.Center.Y - world.DefaultView.Size.Y / 2;
            float bottom = world.DefaultView.Center.Y + world.DefaultView.Size.Y / 2;

            if (player.Bbox.Top - 100 < top)
                world.DefaultView.Move(new Vector2f(0, player.Bbox.Top - 100 - top));
            else if (player.Bbox.Top + player.Bbox.Height + 100 > bottom)
                world.DefaultView.Move(new Vector2f(0, player.Bbox.Top + player.Bbox.Height + 100 - bottom));

            world.SetView(world.DefaultView);
        }
		
		public void readIncomingMsg(){
			NetIncomingMessage msg;
			while ((msg = client.ReadMessage()) != null)
			{
			    switch (msg.MessageType)
			    {
			        case NetIncomingMessageType.VerboseDebugMessage:
			        case NetIncomingMessageType.DebugMessage:
			        case NetIncomingMessageType.WarningMessage:
				    case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
			            break;					
					case NetIncomingMessageType.Data:
						readPacket(msg);
						break;
			        default:
			            Console.WriteLine("Unhandled type: " + msg.MessageType);
			            break;
			    }
			    client.Recycle(msg);
			}
		}
		
		public void readPacket(NetIncomingMessage msg){
		}
    }
}
