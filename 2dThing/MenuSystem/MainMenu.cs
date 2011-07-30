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
		public MainMenu (RenderWindow window) : base()
		{
			this.window = window;
			main = new MenuItem("Main menu bitches", new Vector2f(0,0), null);
		}
		
		public override int Run ()
		{
			
			while(window.IsOpened()){
				window.DispatchEvents();				
				if(Keyboard.IsKeyPressed(Keyboard.Key.L))
					return Screen.GAME;
							
				window.Clear();
				main.Draw(window);
				window.Display();
				Thread.Sleep(10);	
				
			}
			return -1;
		}
	}
}

