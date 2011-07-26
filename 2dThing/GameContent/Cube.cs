using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using System.Collections;

namespace _2dThing.GameContent
{
    public class Cube : Entity
    {
		public static int WIDTH = 32;
		public static int HEIGHT = 32;
		public static int BLOCKTYPECOUNT = 48;
		private static Image tileMap = new Image("content/tilemap.png");
		public int type = 0;
		
        public Cube(int blocktype)
            : base()
        {
			Sprite = new Sprite(tileMap);
			setType(blocktype);
        }
		
		public void setType(int type){
			this.type = type;
			int x = (type % 16) * WIDTH;
			int y = (type / 16) * HEIGHT;			
			Sprite newSprite = new Sprite(new Image("content/tilemap.png"));
			newSprite.SubRect = new IntRect(x, y, WIDTH, HEIGHT);
			Sprite = newSprite;
			
		}
    }
}
