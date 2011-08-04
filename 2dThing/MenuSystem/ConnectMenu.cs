using System;
using SFML.Graphics;
using System.Threading;
using SFML.Window;
using _2dThing.GameContent;

namespace _2dThing
{
	public class ConnectMenu : Screen
	{
		bool returnToMain = false;
		Server server;
		Client client;
		Player p;
		Random randomiser;
		RenderImage pImage;
		Sprite mouse;
		MenuItem title;
		MenuItem ip;
		MenuItem connecting;
		bool enterPressed = false;
		public ConnectMenu(RenderWindow window, ImageManager imageManager, Client client, Server server) : base(window, imageManager)
		{
			this.server = server;
			this.client = client;
			p = new Player(null, imageManager);
			randomiser = new Random();
			pImage = new RenderImage(window.Width, window.Height);
			pImage.DefaultView.Zoom(0.08F);
			pImage.DefaultView.Center = p.Center + new Vector2f(0,-40);
			pImage.DefaultView.Rotate(135);
			pImage.SetView(pImage.DefaultView);
			mouse = new Sprite(imageManager.GetImage("mouse"));
			title = new MenuItem("Enter an IP to connect to...", new Vector2f(0,100), null);
			title.CenterX((int) window.Width);
			ip = new MenuItem("", new Vector2f(0, 150), null);
			ip.CenterX((int) window.Width);
			connecting = new MenuItem("Connecting...", new Vector2f(0,180), null);
			connecting.CenterX((int) window.Width);
			
		}
		
		public override int Run ()
		{
			byte[] rgb = new byte[3];
			randomiser.NextBytes(rgb);
			p.Color = new Color(rgb[2], rgb[1], rgb[0]);
			
			while(true){
				enterPressed = false;
				window.DispatchEvents();
				
				if (returnToMain)
				{
					returnToMain = false;
					return Screen.MAINMENU;
				}
				
				if (enterPressed)
				{
					connecting.Draw(window, false);
					window.Display();
					if(client.isRunning())
						client.Disconnect();
					if(server.isRunning())
						server.stop();
					client.IP = ip.Item;
					client.Connect();
					return Screen.GAME;
				}
				
				window.Clear(new Color(100, 149, 237));
				
				pImage.Clear(new Color(100, 149, 237));
				p.Draw(pImage);
				pImage.Display();					
				
					
				window.Draw(new Sprite(pImage.Image));
				title.Draw(window, false);
				ip.Draw(window, true);				
				window.Draw(mouse);
				
				window.Display();
				
				Thread.Sleep(10);
			}
			
		}
		
		public override void loadEventHandler ()
		{
			base.loadEventHandler ();
			window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
			window.TextEntered += new EventHandler<TextEventArgs>(OnTextEntered);
		}
		
		public override void unloadEventHandler ()
		{
			base.unloadEventHandler ();
			window.KeyPressed -= new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.MouseMoved -= new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
			window.TextEntered -= new EventHandler<TextEventArgs>(OnTextEntered);
		}
		
		private void OnKeyPressed (object sender, EventArgs e)
		{
			
			KeyEventArgs a = (KeyEventArgs)e;
			switch(a.Code){
			case Keyboard.Key.Escape:
				returnToMain = true;
				break;
			case Keyboard.Key.Return:
				enterPressed = true;
				break;
			default:
				break;
			}
			
		}
		
		void OnTextEntered(object sender, EventArgs e){
			
			TextEventArgs a = (TextEventArgs) e;
			
			if(checkUnicode(a.Unicode))
				ip.Item += a.Unicode;
			else if(a.Unicode.Equals("\b") && ip.Item.Length > 0)				
					ip.Item = ip.Item.Substring(0, ip.Item.Length -1);
			
			ip.CenterX((int) window.Width);
			
		}
		
		void OnMouseMoved (object sender, EventArgs e)
		{
			
			MouseMoveEventArgs a = (MouseMoveEventArgs)e;
			mouse.Position = new Vector2f(a.X, a.Y);
			p.lookAt(pImage.ConvertCoords((uint)a.X, (uint)a.Y));
			
		}
		
		bool checkUnicode(String unicode){
			
			return unicode.Equals("0") || unicode.Equals("1") || unicode.Equals("2") || unicode.Equals("3") || unicode.Equals("4") 
					|| unicode.Equals("5") || unicode.Equals("6") || unicode.Equals("7") || unicode.Equals("8") || unicode.Equals("9") || unicode.Equals(".");
		}
		
		
	}
}

