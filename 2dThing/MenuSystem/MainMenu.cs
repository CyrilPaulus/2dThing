using System;
using SFML.Graphics;
using SFML.Window;
using System.Threading;
using _2dThing.GameContent;

namespace _2dThing
{
	public class MainMenu : Screen
	{
		Player p;
		RenderImage pImage;		
		MenuItem[] items;
		bool running = true;
		Client client;
		Server server;
		int selectedIndex = 0;
		const int nbrItem = 4;		
		Sprite mouse;
		Random randomiser = new Random();
		
		Color background = Color.Black;
		Color nextColor = Color.Black;
		
		public MainMenu (RenderWindow window, ImageManager imageManager, Client client, Server server) : base(window, imageManager)
		{
			imageManager = new ImageManager();
			p = new Player(null, imageManager);
			mouse = new Sprite(imageManager.GetImage("mouse"));
			pImage = new RenderImage(window.Width, window.Height);
			pImage.DefaultView.Zoom(0.08F);
			pImage.DefaultView.Center = p.Center + new Vector2f(0,-30);
			pImage.SetView(pImage.DefaultView);
			p.Position = new Vector2f(0,0);
			
			this.window = window;
			this.client = client;
			this.server = server;
			items = new MenuItem[nbrItem];
			items[0] = new MenuItem("Local Game", new Vector2f(0,100), startLocal);
			items[1] = new MenuItem("Connect", new Vector2f(0,130), connect);
			items[2] = new MenuItem("Options", new Vector2f(0, 160), option);
			items[3] = new MenuItem("Exit", new Vector2f(0,220), exit);
			
			foreach(MenuItem i in items)
				i.CenterX((int) window.Width);
			
		}
		
		public override int Run ()
		{
			
			while(running){
				window.DispatchEvents();
				
				int index = 0;
				foreach(MenuItem i in items){					
					if(i.Bbox.Contains(mouse.Position.X, mouse.Position.Y)){
						selectedIndex = index;
						break;
					}
					index++;
				}
				
				if(Keyboard.IsKeyPressed(Keyboard.Key.Return))
					return items[selectedIndex].doAction();
				
				if(Mouse.IsButtonPressed(Mouse.Button.Left)){
					foreach(MenuItem i in items){					
						if(i.Bbox.Contains(mouse.Position.X, mouse.Position.Y))
							return i.doAction();				
						
					}
				}
				
				updateColor();
							
				window.Clear(background);
				
				pImage.Clear(background);
				p.Draw(pImage);
				pImage.Display();					
				
					
				window.Draw(new Sprite(pImage.Image));
				
				index = 0;
				foreach(MenuItem i in items){					
					i.Draw(window, index == selectedIndex);					
					index++;
				}
				window.Draw(mouse);
				window.Display();
				Thread.Sleep(10);	
				
			}
			return -1;
		}
		
		
		private int startLocal(){
			if(server.isRunning()){
				client.Disconnect();
			 	server.stop();
			}
			Thread serverThread = new Thread(server.run);
			serverThread.Start();
			client.IP = "localhost";
			client.Connect();
			return Screen.GAME;
		}
			
		private int connect(){
			return Screen.CONNECT;
		}
		
		private int option(){
			return Screen.OPTION;
		}		
		
		private int exit(){
			return Screen.EXIT;
		}
		
		
		public override void loadEventHandler ()
		{
			base.loadEventHandler ();
			window.Closed += new EventHandler (OnClose);
			window.Resized += new EventHandler<SizeEventArgs>(OnWindowResized);
			window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
		}
		
		public override void unloadEventHandler ()
		{
			base.unloadEventHandler ();
			window.Closed -= new EventHandler (OnClose);
			window.Resized -= new EventHandler<SizeEventArgs>(OnWindowResized);
			window.KeyPressed -= new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.MouseMoved -= new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
		}
		
		void OnWindowResized(object sender, EventArgs e){
			SizeEventArgs a = (SizeEventArgs) e;			
			Resize(a.Width, a.Height);		
		}
		
		void OnClose(object sender, EventArgs e){
			running = false;
		}
		
		private void Resize(uint width, uint height){			
			View newView = new View(new FloatRect(0,0, width, height));
			window.SetView(newView);			
		}
		
		private void OnKeyPressed (object sender, EventArgs e)
		{
			
			KeyEventArgs a = (KeyEventArgs)e;
			switch(a.Code){
			case Keyboard.Key.Up:
				selectedIndex = (selectedIndex + nbrItem - 1) % nbrItem;
				break;
			case Keyboard.Key.Down:
				selectedIndex = (selectedIndex + 1) % nbrItem;
				break;
			default:
				break;
			}
			
		}
		
		void OnMouseMoved (object sender, EventArgs e)
		{
			
			MouseMoveEventArgs a = (MouseMoveEventArgs)e;
			mouse.Position = new Vector2f(a.X, a.Y);
			p.lookAt(pImage.ConvertCoords((uint)a.X, (uint)a.Y));
			
		}
		
		void updateColor(){
			if (background.Equals(nextColor)){
				byte[] rgb = new byte[3];
				randomiser.NextBytes(rgb);
				nextColor = new Color(rgb[0], rgb[1] , rgb[2]);
			}
			
			if(background.R > nextColor.R)
				background.R -= 1;
			else if (background.R < nextColor.R)				
				background.R += 1;
			
			if(background.G > nextColor.G)
				background.G -= 1;
			else if (background.G < nextColor.G)				
				background.G += 1;
			
			if(background.B > nextColor.B)
				background.B -= 1;
			else if (background.B < nextColor.B)				
				background.B += 1;			
			
		}
		
	}
}

