using System;
using SFML.Graphics;

namespace _2dThing {
	public class Screen {
		public static int EXIT = -1;
		public static int MAINMENU = 0;
		public static int GAME = 1;
		public static int CONNECT = 2;
		public static int OPTION = 3;
		public static int LOAD = 4;
		public static int SAVE = 5;
		public static Color BACKGROUND = new Color (100, 149, 237);
		public static Font ARIAL = new Font("content/arial.ttf");
		protected RenderWindow window;
		protected ImageManager imageManager;
		
		public Screen(RenderWindow window, ImageManager imageManager) {
			this.window = window;
			this.imageManager = imageManager;
		}
		
		public virtual int Run() {
			return EXIT;
		}
		
		public virtual void LoadEventHandler() {
		}
		
		public virtual void UnloadEventHandler() {
		}
	}
}

