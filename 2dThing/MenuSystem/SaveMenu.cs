using System;
using SFML.Graphics;
using SFML.Window;

namespace _2dThing {
	public class SaveMenu : Screen {
		private Client client;
		private bool running = true;
		private bool returnToMainMenu = false;
		private bool returnToGame = false;
		private bool enterPressed = false;
		private FileLister fl;
		
		public SaveMenu(RenderWindow window, ImageManager imageManger, Client client) : base(window, imageManger) {
			this.client = client;
			fl = new FileLister(".", false, "*.map");
		}
		
		public override int Run() {
			Resize(window.Width, window.Height);
			while(running){
				enterPressed = false;
				window.DispatchEvents();
				
				if(returnToMainMenu) {
					returnToMainMenu = false;
					return Screen.MAINMENU;
				}
				
				if(returnToGame) {
					returnToGame = false;
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
			window.TextEntered += new EventHandler<TextEventArgs>(OnTextEntered);
		}
		
		public override void UnloadEventHandler() {
			window.Closed -= new EventHandler(OnClose);
			window.KeyPressed -= new EventHandler<KeyEventArgs>(OnKeyPressed);
			window.Resized -= new EventHandler<SizeEventArgs>(OnWindowResized);
			window.TextEntered -= new EventHandler<TextEventArgs>(OnTextEntered);
		}
		
		private void OnKeyPressed(object sender, EventArgs e) {
			enterPressed = true;
			if(!fl.IsEditing()){
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
							int result = fl.Expand();
							if(result == FileLister.NEWFILE){								
								fl.EnterEditMode(FileLister.NEWFILE);
							}
						
							else if(result == FileLister.NEWDIRECTORY){
								fl.EnterEditMode(FileLister.NEWDIRECTORY);
							}
						
							else if(result == FileLister.FILE){
								client.SaveMap(fl.GetSelectedIndex());
								returnToGame = true;
							}
						break;
					case Keyboard.Key.Delete:
							fl.Delete();
						break;
									
					default:
						break;
				}
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
		
		private void OnTextEntered(object sender, EventArgs e) {
			
			TextEventArgs a = (TextEventArgs)e;
			
			if(fl.IsEditing())
				fl.AddChar(a.Unicode);
			
		}
	}
}

