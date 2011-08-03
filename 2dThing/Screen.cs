using System;
using SFML.Graphics;

namespace _2dThing
{
	public class Screen
	{
		public Screen (){}
		public static int EXIT = -1;
		public static int MAINMENU = 0;
		public static int GAME = 1;
		public static int CONNECT = 2;
		public static int OPTION = 3;
		
		public virtual int Run(){
				return 0;
		}
		
		public virtual void loadEventHandler(){
		}
		
		public virtual void unloadEventHandler(){
		}
	}
}

