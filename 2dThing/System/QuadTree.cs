using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using _2dThing.System.GameContent;

namespace _2dThing.System.System
{
    class QuadTree
    {
        int capacity;
        Vector2f minSize;
        QuadTreeNode root;

        public QuadTree(int capacity, Vector2f minSize)
        {
            this.capacity = capacity;
            this.minSize = minSize;
            this.root = new QuadTreeNode(new FloatRect(-1000000, -1000000, 2000000, 2000000), capacity, minSize);
        }

        public void addCube(Cube b)
        {
            root.addCube(b);
        }

        public void removeCube(Cube b)
        {
            root.remove(b);
        }

        public List<Cube> getList(FloatRect bbox)
        {
            return root.getList(bbox);
        }

        /* public void Draw(SpriteBatch sb, Camera cam)
         {
             root.Draw(sb, cam);
         }*/
    }

    class QuadTreeNode
    {
        FloatRect range;
        List<Cube> cubeList;
        List<QuadTreeNode> child;
        int capacity;
        Vector2f minSize;

        public QuadTreeNode(FloatRect range, int capacity, Vector2f minSize)
        {
            this.range = range;
            this.capacity = capacity;
            this.child = null;
            this.cubeList = new List<Cube>();
            this.minSize = minSize;
        }

        public void addCube(Cube b)
        {
            if (child != null)
            {
                foreach (QuadTreeNode c in child)
                {
                    if (c.range.Intersects(b.Bbox))
                    {
                        c.addCube(b);
                    }
                }
            }
            else if (cubeList.Count < capacity)
            {
                cubeList.Add(b);
            }
            else
            {
                this.explode();
                this.addCube(b);
            }
        }

        public void explode()
        {
            if (range.Width <= minSize.X || range.Height <= minSize.Y)
                return;

            child = new List<QuadTreeNode>();
            float width = range.Width / 2;
            float height = range.Height / 2;
            child.Add(new QuadTreeNode(new FloatRect(range.Left, range.Top, width, height), capacity, minSize));
            child.Add(new QuadTreeNode(new FloatRect(range.Left + width, range.Top, width, height), capacity, minSize));
            child.Add(new QuadTreeNode(new FloatRect(range.Left, range.Top + height, width, height), capacity, minSize));
            child.Add(new QuadTreeNode(new FloatRect(range.Left + width, range.Top + height, width, height), capacity, minSize));
            foreach (Cube b in cubeList)
            {
                this.addCube(b);
            }
            cubeList = null;
        }

        public void remove(Cube b)
        {
            if (child != null)
            {
                foreach (QuadTreeNode c in child)
                {
                    if (c.range.Intersects(b.Bbox))
                        c.remove(b);
                }
            }
            else
                cubeList.Remove(b);
        }

        public List<Cube> getList(FloatRect bbox)
        {
            if (child != null)
            {
                List<Cube> ret = new List<Cube>();
                foreach (QuadTreeNode c in child)
                {
                    if (c.range.Intersects(bbox))
                    {
                        ret = ret.Concat<Cube>(c.getList(bbox)).ToList();

                    }
                }
                return ret;
            }
            else
                return cubeList;

        }

        /* public void Draw(SpriteBatch sb, Camera cam)
         {
             if (child == null)
                 C3.XNA.Primitives2D.DrawRectangle(sb, cam.globalToLocal(range), Color.Green);
             else
                 foreach (QuadTreeNode c in child)
                 {
                     c.Draw(sb, cam);
                 }
         }*/

    }

}
