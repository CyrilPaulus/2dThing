using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace _2dThing.GameContent
{
    public class Cube : Entity
    {
        public Cube()
            : base()
        {
            Sprite = new Sprite(new Image("content/cube.png"));            
        }
    }
}
