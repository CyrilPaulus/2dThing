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

        float speed = 100;
        float fallSpeed = 1;
		float jumpAcc = 300;
        bool noclip = true;

        bool flying = false;		

        public Player(World world)
            : base()
        {
            this.world = world;
            sprite = new Sprite(new Image("content/player.png"));
            leftPupil = new Sprite(new Image("content/pupil.png"));
            rightPupil = new Sprite(new Image("content/pupil.png"));

            leftPupilOrigin = new Vector2f(5, 9);
            rightPupilOrigin = new Vector2f(20, 9);            

            leftPupilPosition = leftPupilOrigin;
            rightPupilPosition = rightPupilOrigin;            
        }

        public override void Draw(RenderTarget world)
        {
            base.Draw(world);
            leftPupil.Position = Position + leftPupilPosition;
            rightPupil.Position = Position + rightPupilPosition;
            world.Draw(leftPupil);
            world.Draw(rightPupil);
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


            if (input.Left)
            {
                Position += new Vector2f(-speed, 0) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox);
                if (colliding != null)
                    Position = new Vector2f(colliding.Bbox.Left + colliding.Bbox.Width, Position.Y);
            }

            if (input.Right)
            {
                Position += new Vector2f(speed, 0) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox);
                if (colliding != null)
                    Position = new Vector2f(colliding.Bbox.Left - Bbox.Width, Position.Y);
            }

            if (input.Up && noclip)
            {
                Position += new Vector2f(0, -speed) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox);
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
                Cube colliding = world.getCollidingCube(Bbox);
                if (colliding != null)
                    Position = new Vector2f(Position.X, colliding.Bbox.Top - Bbox.Height);
            }

            if (!noclip)
            {
                Position += new Vector2f(0, fallSpeed) * frameTime;
               	fallSpeed = Math.Min(fallSpeed + world.Gravity, world.MaxFallSpeed);
                Cube colliding = world.getCollidingCube(Bbox);
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
            }
        }

       	public Vector2f Center 
		{
			get { return Position + new Vector2f(Bbox.Width / 2, Bbox.Height / 2); }
		}
    }
}
