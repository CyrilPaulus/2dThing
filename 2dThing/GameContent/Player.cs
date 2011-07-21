using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using _2dThing.System.System;

namespace _2dThing.System.GameContent
{
    class Player : Entity
    {
        World world;

        Vector2f leftPupilOrigin;
        Vector2f rightPupilOrigin;
        Vector2f leftPupilPosition;
        Vector2f rightPupilPosition;        
        Sprite leftPupil;
        Sprite rightPupil;

        float speed = 100;
        float acceleration = 1;
        bool noclip = false;

        bool moveLeft = false;
        bool moveRight = false;
        bool moveUp = false;
        bool moveDown = false;

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

        public override void update(float frameTime)
        {
            base.update(frameTime);


            if (moveLeft)
            {
                Position += new Vector2f(-speed, 0) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox);
                if (colliding != null)
                    Position = new Vector2f(colliding.Bbox.Left + colliding.Bbox.Width, Position.Y);
            }

            if (moveRight)
            {
                Position += new Vector2f(speed, 0) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox);
                if (colliding != null)
                    Position = new Vector2f(colliding.Bbox.Left - Bbox.Width, Position.Y);
            }

            if (moveUp && noclip)
            {
                Position += new Vector2f(0, -speed) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox);
                if (colliding != null)
                    Position = new Vector2f(Position.X, colliding.Bbox.Top + colliding.Bbox.Height);
            }
            else if (moveUp && !noclip && !flying)
            {
                acceleration = -2.5f;                
            }

            if (moveDown)
            {
                Position += new Vector2f(0, speed) * frameTime;
                Cube colliding = world.getCollidingCube(Bbox);
                if (colliding != null)
                    Position = new Vector2f(Position.X, colliding.Bbox.Top - Bbox.Height);
            }

            if (!noclip)
            {
                Position += new Vector2f(0, world.Gravity * acceleration) * frameTime;
                acceleration += 0.2f;
                Cube colliding = world.getCollidingCube(Bbox);
                flying = true;
                if (colliding != null)
                {
                    if (acceleration > 0)
                    {
                        Position = new Vector2f(Position.X, colliding.Bbox.Top - Bbox.Height);
                        flying = false;
                    }
                    else
                        Position = new Vector2f(Position.X, colliding.Bbox.Top + colliding.Bbox.Height);
                    acceleration = 0;        

                    
                }
            }
        }

        public bool Left
        {
            get { return moveLeft; }
            set { moveLeft = value; }
        }

        public bool Right
        {
            get { return Right; }
            set { moveRight = value; }
        }
        public bool Up
        {
            get { return Up; }
            set { moveUp = value; }
        }
        public bool Down
        {
            get { return Down; }
            set { moveDown = value; }
        }
    }
}
