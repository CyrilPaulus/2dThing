using System;
using SFML.Graphics;
using SFML.Window;
using System.IO;

namespace _2dThing {
	public class LoadMenu : Screen{
		private Server server;
		private bool running = true;
		private bool returnToMainMenu = false;
		private bool returnToGame = false;
		private FileLister fl;
		
		public LoadMenu(RenderWindow window, ImageManager imageManger, Server server) : base(window, imageManger) {
			this.server = server;
			fl = new FileLister(".", true, "*.map");
		}
		
		public override int Run() {
			Resize(window.Width, window.Height);
			while(running){
				window.DispatchEvents();
				
				if(returnToMainMenu) {
					returnToMainMenu = false;
					return Screen.MAINMENU;
				}
				
				if(returnToGame) {
					returnToMainMenu = false;
					return Screen.GAME;
				}
				
				window.Clear(Screen.BACKGROUND);
				fl.Draw(window);
				window.Display();
			}
			return -1;
		}
		
		public override void LoadEventHandler() {
			window.Closed += new EventHandler(OnClose);
			window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.Resized += new EventHandler<SizeEventArgs>(OnWindowResized);
		}
		
		public override void UnloadEventHandler() {
			window.Closed -= new EventHandler(OnClose);
			window.KeyPressed -= new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.Resized -= new EventHandler<SizeEventArgs>(OnWindowResized);
		}
		
		private void OnKeyPressed(object sender, EventArgs e) {
			
			KeyEventArgs a = (KeyEventArgs)e;
			switch (a.Code) {				
				case Keyboard.Key.Escape:
						returnToMainMenu = true;
					break;
				case Keyboard.Key.Back:
						fl.Parent();
					break;
				case Keyboard.Key.Up:
						fl.Up();
					break;
				case Keyboard.Key.Down:
						fl.Down();
					break;
				case Keyboard.Key.Return:
						if(fl.Expand() == FileLister.FILE){
							server.LoadMap(fl.GetSelectedIndex());
							returnToGame = true;
						}
					break;
								
				default:
					break;
			}
			
		}
		
		private void OnClose(object sender, EventArgs e) {
			running = false;
		}
		
		private void OnWindowResized(object sender, EventArgs e) {
			SizeEventArgs a = (SizeEventArgs)e;			
			Resize(a.Width, a.Height);		
		}
		
		private void Resize(uint width, uint height) {			
			View newView = new View(new FloatRect(0, 0, width, height));
			window.SetView(newView);					
		}
	}
}

