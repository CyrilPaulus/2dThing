using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _2dThing.Utils;
using SFML.Window;
using SFML.Graphics;

namespace _2dThing.GameContent
{
    public class World
    {
        List<Cube> cubeList;
        List<Player> playerList;
        float gravity = 600;
		float maxFallSpeed = 2000;

        QuadTree quadTree;

        public World()
        {
            playerList = new List<Player>();
            cubeList = new List<Cube>();
            quadTree = new QuadTree(10, new Vector2f(90, 90));
        }

        public bool addCube(Vector2f pos, int type)
        {
            Vector2f gridPos = new Vector2f((float)Math.Floor((pos.X / Cube.WIDTH)) * Cube.WIDTH, (float)Math.Floor((pos.Y / Cube.HEIGHT)) * Cube.HEIGHT);
            Cube cube = new Cube(type);
            cube.Position = gridPos;
            bool exist = false;

            foreach(Cube c in quadTree.getList(cube.Bbox))
                if(c.Bbox.Intersects(cube.Bbox))
                    exist = true;

            foreach (Player p in playerList)
                if (p.Bbox.Intersects(cube.Bbox))
                    exist = true;

            if (!exist)
            {
                cubeList.Add(cube);
                quadTree.addCube(cube);
            }
			return !exist;
        }
		
		public void forceAddCube(Vector2f pos, int type){
			Vector2f gridPos = new Vector2f((float)Math.Floor((pos.X / Cube.WIDTH)) * Cube.WIDTH, (float)Math.Floor((pos.Y / Cube.HEIGHT)) * Cube.HEIGHT);
            Cube cube = new Cube(type);
            cube.Position = gridPos;
			cubeList.Add(cube);
            quadTree.addCube(cube);
		}
		
		

        public void deleteCube(Vector2f pos)
        {
            foreach (Cube c in cubeList)
            {
                if (c.Bbox.Contains(pos.X, pos.Y))
                {
                    cubeList.Remove(c);
                    quadTree.removeCube(c);
                    return;
                }
            }
        }

        public void addPlayer(Player p)
        {
            playerList.Add(p);           
        }

        public void deletePlayer(Player p)
        {
            playerList.Remove(p);
        }

        public void Draw(RenderTarget rt)
        {
						
            foreach (Cube c in cubeList)
                c.Draw(rt);

            foreach (Player p in playerList)                
                p.Draw(rt);

        }
		
		public void DrawDebug(RenderTarget rt){
			
			
			foreach(Cube c in cubeList)
				c.DrawDebug(rt);
				
			foreach(Player p in playerList){
				p.DrawDebug(rt);
				foreach (Cube c in quadTree.getList(p.Bbox))
					c.DrawDebug(rt, 215);
			}
		}

        public Cube getCollidingCube(FloatRect bbox)
        {
            foreach (Cube c in quadTree.getList(bbox))
            {
                if (c.Bbox.Intersects(bbox))
                    return c;
            }
            return null;
        }

        public List<Player> Players
        {
            get { return playerList; }
        }

        public float Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }
		
		public float MaxFallSpeed
		{
			get{ return maxFallSpeed;}
			set{ maxFallSpeed = value;}
		}
		
		public List<Cube> CubeList{
			get { return cubeList; }
		}

    }
}
