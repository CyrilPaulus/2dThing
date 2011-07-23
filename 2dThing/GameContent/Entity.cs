using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace _2dThing.GameContent
{
    public class Entity
    {
        protected Sprite sprite;
		protected Vector2f offset;
		protected Vector2f position;
		protected Vector2f size;
        public Entity()
        {            
            Sprite = new Sprite(new Image("content/cube.png"));
			offset = new Vector2f(0, 0);
			position = new Vector2f(0, 0);			
        }

        public Vector2f Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2f Scale
        {
            get { return sprite.Scale; }
            set { sprite.Scale = value; }
        }        

        public virtual void Draw(RenderTarget world)
        {
			sprite.Position = Position + offset;
          	world.Draw(sprite);
          	/*Shape rectangle = Shape.Rectangle(Bbox, new Color(0, 0, 0, 0), -2, Color.Red);
          	world.Draw(rectangle); */           
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

        public virtual FloatRect Bbox
        {
            get { return new FloatRect(Position.X, Position.Y, size.X, size.Y); }
        }
		
		public Sprite Sprite{
			set { sprite = value; size = new Vector2f(sprite.Width, sprite.Height);}
		}
    
    }
}
