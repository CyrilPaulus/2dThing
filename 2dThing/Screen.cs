using System;
using SFML.Graphics;

namespace _2dThing
{
	public class Screen
	{
		public Screen (){}
		public static int MAINMENU = 0;
		public static int GAME = 1;
		
		public virtual int Run(){
				return 0;
		}
	}
}

