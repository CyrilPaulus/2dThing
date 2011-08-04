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
		
		bool returnToGame = false;
		bool mouseButtonPressed = false;
		bool enterPressed = false;
		public MainMenu (RenderWindow window, ImageManager imageManager, Client client, Server server) : base(window, imageManager)
		{
			imageManager = new ImageManager();
			p = new Player(null, imageManager);
			mouse = new Sprite(imageManager.GetImage("mouse"));
			pImage = new RenderImage(window.Width, window.Height);
			pImage.DefaultView.Zoom(0.08F);
			pImage.DefaultView.Center = p.Center + new Vector2f(0,-30);
			pImage.SetView(pImage.DefaultView);			
			
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
			byte[] rgb = new byte[3];
			randomiser.NextBytes(rgb);
			p.Color = new Color(rgb[2], rgb[1], rgb[0]);
			Resize(window.Width, window.Height);
			while(running){
				mouseButtonPressed = false;
				enterPressed = false;
				window.DispatchEvents();
				
				if(returnToGame){
					returnToGame = false;
					return Screen.GAME;
				}
				
				int index = 0;
				foreach(MenuItem i in items){					
					if(i.Bbox.Contains(mouse.Position.X, mouse.Position.Y)){
						selectedIndex = index;
						break;
					}
					index++;
				}
				
				if(enterPressed)
					return items[selectedIndex].doAction();
				
				if(mouseButtonPressed){
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
				
				Sprite pSprite = new Sprite(pImage.Image);
				pSprite.Position = new Vector2f(((int)window.Width - (int)pImage.Width) / 2 , (int)window.Height - (int)pImage.Height);
				window.Draw(pSprite);
				
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
		
		
		public override void LoadEventHandler ()
		{
			base.LoadEventHandler ();
			window.Closed += new EventHandler (OnClose);
			window.Resized += new EventHandler<SizeEventArgs>(OnWindowResized);
			window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
			window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);			
		}
		
		public override void UnloadEventHandler ()
		{
			base.UnloadEventHandler ();
			window.Closed -= new EventHandler (OnClose);
			window.Resized -= new EventHandler<SizeEventArgs>(OnWindowResized);
			window.KeyPressed -= new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.MouseMoved -= new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
			window.MouseButtonPressed -= new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);			
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
				
			foreach(MenuItem i in items)
				i.CenterX((int) width);
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
			case Keyboard.Key.Escape:
				if(client.isRunning())
					returnToGame = true;
				break;
			case Keyboard.Key.Return:
				enterPressed = true;
				break;
			default:
				break;
			}
			
		}
		
		void OnMouseMoved (object sender, EventArgs e)
		{
			
			MouseMoveEventArgs a = (MouseMoveEventArgs)e;
			mouse.Position = new Vector2f(a.X, a.Y);
			p.LookAt(pImage.ConvertCoords((uint)a.X, (uint)a.Y));
			
		}
		
		void OnMouseButtonPressed(object sender, EventArgs e)
		{
			MouseButtonEventArgs a = (MouseButtonEventArgs) e;
			if (a.Button == Mouse.Button.Left)
				mouseButtonPressed = true;
		}		
				
		void updateColor()
		{
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

