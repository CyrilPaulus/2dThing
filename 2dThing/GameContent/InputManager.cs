using System;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using _2dThing.GameContent;

namespace _2dThing
{
	public class InputManager
	{
		private Input input;
		private Client client;		
		private Dictionary<Keyboard.Key, Action<bool>> keyMap;
		private bool mainMenu = false;		
		
		public InputManager (Client client)		
		{
			this.client = client;
			RenderWindow window = client.MainWindow;
			input = new Input();			
			
			keyMap = new Dictionary<Keyboard.Key, Action<bool>>();
			
			keyMap[Keyboard.Key.Up] = moveUp;
			keyMap[Keyboard.Key.Down] = moveDown;
			keyMap[Keyboard.Key.Left] = moveLeft;
			keyMap[Keyboard.Key.Right] = moveRight;
			keyMap[Keyboard.Key.R] = resetPlayer;
			keyMap[Keyboard.Key.PageUp] = zoomIn;
			keyMap[Keyboard.Key.PageDown] = zoomOut;
			keyMap[Keyboard.Key.A] = layerIn;
			keyMap[Keyboard.Key.Z] = layerOut;
			keyMap[Keyboard.Key.N] = toggleNoclip;
			keyMap[Keyboard.Key.Escape] = toMainMenu;
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
		
		private void moveUp(bool pressed){
			input.Up = pressed;
		}
		
		private void moveDown(bool pressed){
			input.Down = pressed;
		}
		
		private void moveLeft(bool pressed){
			input.Left = pressed;
		}
		
		private void moveRight(bool pressed){
			input.Right = pressed;
		}
		
		private void resetPlayer(bool pressed){
			if(pressed){
				client.Player.reset();
				ClientReset cr = new ClientReset(client.ClientId);
				client.SendPacket(cr);
			}
		}
		
		private void zoomIn(bool pressed){
			if(pressed)
				client.Zoom(1.3333333F);
		}
		
		private void zoomOut(bool pressed){
			if(pressed)
				client.Zoom(0.75F);
		}
		
		private void layerOut(bool pressed){
			if(pressed)
				client.Player.Layer++;
			input.UpperLayer = pressed;
		}
		
		private void layerIn(bool pressed){
			if(pressed)
				client.Player.Layer--;
		}
		
		private void toMainMenu(bool pressed){
			if(pressed)
				mainMenu = true;
		}
		
		private void toggleNoclip(bool pressed){
			if(pressed)
				client.Player.Noclip = !client.Player.Noclip;
		}

		void OnKeyPressed (object sender, EventArgs e)
		{
			
			KeyEventArgs a = (KeyEventArgs)e;
			if(!client.Chat.Writing)
			try
			{
				keyMap[a.Code](true);
			} 
			catch(KeyNotFoundException exp)
			{
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
			if(!client.Chat.Writing)
			try
			{
				keyMap[a.Code](false);
			} 
			catch(KeyNotFoundException exp)
			{
			}	
			
		}
		
		void OnMouseWheelMoved(object sender, EventArgs e){
			
			MouseWheelEventArgs a = (MouseWheelEventArgs) e;
			if(a.Delta < 0)
				client.BlockType = (client.BlockType - 1 + Cube.BLOCKTYPECOUNT) % Cube.BLOCKTYPECOUNT;
			else
				client.BlockType = (client.BlockType + 1) % Cube.BLOCKTYPECOUNT;
			
		}
		
		public Input Input{
			get {return input;}
		}
		
		public bool MainMenu{
			get { return mainMenu;}
			set { mainMenu = value; }
		}
		
			
		public void loadEventHandler(){
			RenderWindow window = client.MainWindow;
			window.MouseMoved += new EventHandler<MouseMoveEventArgs> (OnMouseMoved);
			window.KeyPressed += new EventHandler<KeyEventArgs> (OnKeyPressed);
			window.KeyReleased += new EventHandler<KeyEventArgs> (OnKeyReleased);
			window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs> (OnMouseButtonPressed);
			window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseButtonReleased);
			window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs> (OnMouseWheelMoved);
			window.TextEntered += new EventHandler<TextEventArgs>(OnTextEntered);
		}
		
		public void unloadEventHandler(){
			RenderWindow window = client.MainWindow;
			window.MouseMoved -= new EventHandler<MouseMoveEventArgs> (OnMouseMoved);
			window.KeyPressed -= new EventHandler<KeyEventArgs> (OnKeyPressed);
			window.KeyReleased -= new EventHandler<KeyEventArgs> (OnKeyReleased);
			window.MouseButtonPressed -= new EventHandler<MouseButtonEventArgs> (OnMouseButtonPressed);
			window.MouseButtonReleased -= new EventHandler<MouseButtonEventArgs>(OnMouseButtonReleased);
			window.MouseWheelMoved -= new EventHandler<MouseWheelEventArgs> (OnMouseWheelMoved);
			window.TextEntered -= new EventHandler<TextEventArgs>(OnTextEntered);
		}
	}
}

