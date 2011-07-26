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
		
        public Cube(int blocktype)
            : base()
        {
			Sprite = new Sprite(new Image("content/tilemap.png"));
			setType(blocktype);
        }
		
		public void setType(int type){
			int x = (type % 16) * WIDTH;
			int y = (type / 16) * HEIGHT;			
			Sprite newSprite = new Sprite(new Image("content/tilemap.png"));
			newSprite.SubRect = new IntRect(x, y, WIDTH, HEIGHT);
			Sprite = newSprite;
			
		}
    }
}
