using System;
using SFML.Graphics;
using SFML.Window;

namespace _2dThing
{
	public class InputManager
	{
		private Input input;
		private Client client;		
		
		
		public InputManager (Client client)		
		{
			this.client = client;
			RenderWindow window = client.MainWindow;
			input = new Input();
			
			
			window.MouseMoved += new EventHandler<MouseMoveEventArgs> (OnMouseMoved);
			window.KeyPressed += new EventHandler<KeyEventArgs> (OnKeyPressed);
			window.KeyReleased += new EventHandler<KeyEventArgs> (OnKeyReleased);
			window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs> (OnMouseButtonPressed);
			window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseButtonReleased);
			window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs> (OnMouseWheelMoved);
			window.TextEntered += new EventHandler<TextEventArgs>(OnTextEntered);
		}
		
		void OnMouseMoved (object sender, EventArgs e)
		{
			MouseMoveEventArgs a = (MouseMoveEventArgs)e;
			client.Mouse.Position = new Vector2f (a.X, a.Y);            
		}

		void OnMouseButtonPressed (object sender, EventArgs e)
		{
			MouseButtonEventArgs a = (MouseButtonEventArgs)e;
			switch(a.Button){
			case Mouse.Button.Left:
				input.LeftMouseButton = true;
				break;
			case Mouse.Button.Right:
				input.RightMouseButton = true;
				break;
			default:break;
			}
		}
		
		void OnMouseButtonReleased (object sender, EventArgs e)
		{
			
			MouseButtonEventArgs a = (MouseButtonEventArgs)e;
			switch(a.Button){
			case Mouse.Button.Left:
				input.LeftMouseButton = false;
				break;
			case Mouse.Button.Right:
				input.RightMouseButton = false;
				break;
			default:break;
			}
		}

		void OnKeyPressed (object sender, EventArgs e)
		{
			KeyEventArgs a = (KeyEventArgs)e;
			
			if(!client.Chat.Writing){
				switch (a.Code) {
				case Keyboard.Key.Left:
	                input.Left = true;          
					break;
				case Keyboard.Key.Right:
	                input.Right = true;				
					break;
				case Keyboard.Key.Up:
	                input.Up = true;				
					break;
				case Keyboard.Key.Down:
	                input.Down = true;				
					break;
				case Keyboard.Key.R:
					client.Player.reset();
					ClientReset cr = new ClientReset(client.ClientId);
					client.sendPkt(cr);
					break;				
				case Keyboard.Key.PageDown:
					client.Zoom(1.3333333F);
					break;
				case Keyboard.Key.PageUp:
					client.Zoom(0.75F);
					break;			
				default:
					break;
				}
			}
		}
		
		void OnTextEntered(object sender, EventArgs e){
			TextEventArgs a = (TextEventArgs) e;
			if(!client.Chat.Writing && a.Unicode.Equals("y")){
				client.Chat.Writing = true;
				
			}
			else if (client.Chat.Writing)
			{				
				client.Chat.update(a.Unicode);
			}
			
		}
		
		void OnKeyReleased (object sender, EventArgs e)
		{
			KeyEventArgs a = (KeyEventArgs)e;
			switch (a.Code) {
			case Keyboard.Key.Left:
                input.Left = false;          
				break;
			case Keyboard.Key.Right:
                input.Right = false;				
				break;
			case Keyboard.Key.Up:
                input.Up = false;				
				break;
			case Keyboard.Key.Down:
                input.Down = false;				
				break;			
			default:
				break;
			}
		}
		
		void OnMouseWheelMoved(object sender, EventArgs e){
			MouseWheelEventArgs a = (MouseWheelEventArgs) e;
			if(a.Delta < 0)
				client.Zoom(1.3333333F);
			else
				client.Zoom(0.75F);
		}
		
		public Input Input{
			get {return input;}
		}
	}
}

