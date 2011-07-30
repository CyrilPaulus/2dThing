using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using _2dThing.Utils;

namespace _2dThing.GameContent
{
    public class Player : Entity
    {
        World world;
		
        Vector2f leftPupilOrigin;
        Vector2f rightPupilOrigin;
        Vector2f leftPupilPosition;
        Vector2f rightPupilPosition;        
        Sprite leftPupil;
        Sprite rightPupil;
		Sprite colorMask;

        float speed = 100;
        float fallSpeed = 1;
		float jumpAcc = 200;
        bool noclip = false;
		int layer = World.LAYERNBR - 1;

        bool flying = false;		

        public Player(World world, ImageManager imageManager)
            : base(imageManager)
        {
            this.world = world;
            Sprite = new Sprite(imageManager.GetImage("player"));			
			size = new Vector2f(sprite.Width - 2, sprite.Height);
			offset = new Vector2f(-1,0);
            leftPupil = new Sprite(imageManager.GetImage("pupil"));
            rightPupil = new Sprite(imageManager.GetImage("pupil"));
			colorMask = new Sprite(imageManager.GetImage("colorMask"));
			Random randomiser = new Random();
			byte[] rgb = new byte[3];
			randomiser.NextBytes(rgb);
			colorMask.Color = new Color(rgb[0], rgb[1] , rgb[2]);
			

            leftPupilOrigin = new Vector2f(5, 9);
            rightPupilOrigin = new Vector2f(20, 9);            

            leftPupilPosition = leftPupilOrigin;
            rightPupilPosition = rightPupilOrigin;
			
			
        }
		
		public Color Color
		{
			get { return colorMask.Color; }
			set { colorMask.Color = value; }
		}

        public override void Draw(RenderTarget world)
        {
            base.Draw(world);
            leftPupil.Position = sprite.Position + leftPupilPosition;
            rightPupil.Position = sprite.Position + rightPupilPosition;
			colorMask.Position = sprite.Position;
            world.Draw(leftPupil);
            world.Draw(rightPupil);
			world.Draw(colorMask);			
        }
		
		public override void Draw(RenderTarget world, Color color){
			Color ori = colorMask.Color;
			colorMask.Color = new Color((byte) (ori.R * color.R / 255), (byte) (ori.G * color.G / 255), (byte) (ori.B * Color.B / 255), (byte) (ori.A * Color.A / 255));
			sprite.Color = color;
			leftPupil.Color = color;
			rightPupil.Color = color;
			this.Draw(world);
			colorMask.Color = ori;
			sprite.Color = Color.White;
			leftPupil.Color = Color.White;
			rightPupil.Color = Color.White;
		}

        public void lookAt(Vector2f pos)
        {
            float leftPupilDist = Math.Min(VectorUtils.Distance(pos, sprite.Position + leftPupilOrigin) / 50, 2);
            Vector2f leftPupilDir = pos - (sprite.Position + leftPupilOrigin);
            leftPupilDir = VectorUtils.Normalize(leftPupilDir);
            leftPupilPosition = leftPupilOrigin + (leftPupilDist * leftPupilDir);

            float rightPupilDist = Math.Min(VectorUtils.Distance(pos, sprite.Position + rightPupilOrigin) / 50, 2);
            Vector2f rightPupilDir = pos - (sprite.Position + rightPupilOrigin);
            rightPupilDir = VectorUtils.Normalize(rightPupilDir);
            rightPupilPosition = rightPupilOrigin + (rightPupilDist * rightPupilDir);
        }

        public void update(float frameTime, Input input)
        {
            base.update(frameTime);
			Vector2f oldPos = Position;

            if (input.Left)
            {
                Position += new Vector2f(-speed, 0) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox, layer);
                if (colliding != null)
                    Position = new Vector2f(colliding.Bbox.Left + colliding.Bbox.Width, Position.Y);
            }

            if (input.Right)
            {
                Position += new Vector2f(speed, 0) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox, layer);
                if (colliding != null)
                    Position = new Vector2f(colliding.Bbox.Left - Bbox.Width, Position.Y);
            }

            if (input.Up && noclip)
            {
                Position += new Vector2f(0, -speed) * frameTime;
               	Cube colliding = world.getCollidingCube(Bbox, layer);
                if (colliding != null)
                    Position = new Vector2f(Position.X, colliding.Bbox.Top + colliding.Bbox.Height);
            }
            else if (input.Up && !noclip && !flying)
            {
                fallSpeed = - jumpAcc;                
            }

            if (input.Down && noclip)
            {
                Position += new Vector2f(0, speed) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox, layer);
                if (colliding != null)
                    Position = new Vector2f(Position.X, colliding.Bbox.Top - Bbox.Height);
            }

            if (!noclip)
            {
                Position += new Vector2f(0, fallSpeed) * frameTime;               	
                Cube colliding = world.getCollidingCube(Bbox, layer);
                flying = true;
                if (colliding != null)
                {
                    if (fallSpeed > 0)
                    {
                        Position = new Vector2f(Position.X, colliding.Bbox.Top - Bbox.Height);
                        flying = false;
                    }
                    else
                        Position = new Vector2f(Position.X, colliding.Bbox.Top + colliding.Bbox.Height);
					
                    fallSpeed = 0;
                }
				else
				{
					fallSpeed = Math.Min(fallSpeed + (world.Gravity * frameTime), world.MaxFallSpeed);
				}
            }		
			
			
        }
		
		public Vector2f Center 
		{
			get { return Position + new Vector2f(Bbox.Width / 2, Bbox.Height / 2); }
		}		
		
		public float FallSpeed{
			get {return fallSpeed;}
			set {fallSpeed = value;}
		}
		
		public void reset(){
			Position = new Vector2f(0,0);
			fallSpeed = 0;
		}
		
		public int Layer{
			get { return layer; }
			set 
			{	
				if(value >= 0 && value < World.LAYERNBR && world.getCollidingCube(Bbox, value) == null)
					layer = value;
			}
		}
		
		public bool Noclip{
			get { return noclip; }
			set {noclip = value; }
		}
    }
}
