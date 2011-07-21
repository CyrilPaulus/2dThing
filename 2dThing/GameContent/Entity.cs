using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace _2dThing.GameContent
{
    class Entity
    {
        protected Sprite sprite;

        public Entity()
        {            
            this.sprite = new Sprite(new Image("content/cube.png"));
        }

        public Vector2f Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public Vector2f Scale
        {
            get { return sprite.Scale; }
            set { sprite.Scale = value; }
        }        

        public virtual void Draw(RenderTarget world)
        {
          world.Draw(sprite);
          //Shape rectangle = Shape.Rectangle(Bbox, new Color(0, 0, 0, 0), 2, Color.Green);
          //world.Draw(rectangle);            
        }

        public virtual void DrawUI(RenderTarget ui)
        {
           
        }

        public virtual void update(float frameTime)
        {

        }

        public Image Image
        {
            set {sprite.Image = value;}
        }

        public FloatRect Bbox
        {
            get { return new FloatRect(sprite.Position.X, sprite.Position.Y, sprite.Width, sprite.Height); }
        }
    
    }
}
