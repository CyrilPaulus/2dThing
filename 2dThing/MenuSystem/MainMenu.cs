using System;
using SFML.Graphics;
using SFML.Window;
using System.Threading;

namespace _2dThing
{
	public class MainMenu : Screen
	{
		RenderWindow window;
		MenuItem main;
		bool running = true;
		Client client;
		Server server;
		public MainMenu (RenderWindow window, Client client, Server server) : base()
		{
			this.window = window;
			this.client = client;
			this.server = server;
			main = new MenuItem("Main menu bitches", new Vector2f(0,0), null);
		}
		
		public override int Run ()
		{
			
			while(running){
				window.DispatchEvents();				
				if(Keyboard.IsKeyPressed(Keyboard.Key.L))
					return Screen.GAME;
				
				if(Keyboard.IsKeyPressed(Keyboard.Key.H)){
					Thread serverThread = new Thread(server.run);
					serverThread.Start();
					client.Connect();
					return Screen.GAME;
				}
							
				window.Clear();
				main.Draw(window);
				window.Display();
				Thread.Sleep(10);	
				
			}
			return -1;
		}
		
		public override void loadEventHandler ()
		{
			base.loadEventHandler ();
			window.Closed += new EventHandler (OnClose);
			window.Resized += new EventHandler<SizeEventArgs>(OnWindowResized);	
		}
		
		public override void unloadEventHandler ()
		{
			base.unloadEventHandler ();
			window.Closed -= new EventHandler (OnClose);
			window.Resized -= new EventHandler<SizeEventArgs>(OnWindowResized);	
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
		
	}
}

