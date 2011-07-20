using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace _2dThing.System.GameContent
{
    class Cube : Entity
    {
        public Cube()
            : base()
        {
            sprite = new Sprite(new Image("content/cube.png"));            
        }
    }
}
