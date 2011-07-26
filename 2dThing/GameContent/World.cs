using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using _2dThing.Utils;
using SFML.Window;
using SFML.Graphics;


namespace _2dThing.GameContent
{
    public class World
    {
		public static int LAYERNBR = 4;
        List<List<Cube>> cubeLists;
		List<Player> playerList;
        float gravity = 600;
		float maxFallSpeed = 2000;

        List<QuadTree> quadTrees;

        public World()
        {
            playerList = new List<Player>();            
			cubeLists = new List<List<Cube>>();
			quadTrees = new List<QuadTree>();
			
			for(int i = 0; i < LAYERNBR; i++){
				cubeLists.Add(new List<Cube>());
            	quadTrees.Add(new QuadTree(10, new Vector2f(90, 90)));
			}
        }

        public bool addCube(Vector2f pos, int type, int layer)
        {
            Vector2f gridPos = new Vector2f((float)Math.Floor((pos.X / Cube.WIDTH)) * Cube.WIDTH, (float)Math.Floor((pos.Y / Cube.HEIGHT)) * Cube.HEIGHT);
            Cube cube = new Cube(type);
            cube.Position = gridPos;
            bool exist = false;

            foreach(Cube c in quadTrees[layer].getList(cube.Bbox))
                if(c.Bbox.Intersects(cube.Bbox))
                    exist = true;

            foreach (Player p in playerList)
                if (p.Bbox.Intersects(cube.Bbox))
                    exist = true;

            if (!exist)
            {
                cubeLists[layer].Add(cube);
                quadTrees[layer].addCube(cube);
            }
			return !exist;
        }
		
		public void forceAddCube(Vector2f pos, int type, int layer){
			Vector2f gridPos = new Vector2f((float)Math.Floor((pos.X / Cube.WIDTH)) * Cube.WIDTH, (float)Math.Floor((pos.Y / Cube.HEIGHT)) * Cube.HEIGHT);
            Cube cube = new Cube(type);
            cube.Position = gridPos;
			cubeLists[layer].Add(cube);
            quadTrees[layer].addCube(cube);
		}
		
		

        public void deleteCube(Vector2f pos, int layer)
        {
            foreach (Cube c in cubeLists[layer])
            {
                if (c.Bbox.Contains(pos.X, pos.Y))
                {
                    cubeLists[layer].Remove(c);
                    quadTrees[layer].removeCube(c);
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

        public void Draw(RenderTarget rt, int layer)
        {
			int layerIndex = 0;
			foreach(List<Cube> cubeList in cubeLists){
				if(layerIndex < layer - 1){
            		foreach (Cube c in cubeList)
               			 c.Draw(rt, new Color(150,150,150));
				}
				else if (layerIndex > layer){
					foreach (Cube c in cubeList)
               			 c.Draw(rt, new Color(255, 255, 255, 150));
				}
				else {
					foreach (Cube c in cubeList)
               		 c.Draw(rt);	
				}
				foreach (Player p in playerList)
					if(p.Layer < layer)
                		p.Draw(rt, new Color(150,150,150));
					else if(p.Layer > layer)
						p.Draw(rt, new Color(255,255,255,150));
					else
						p.Draw(rt);
				layerIndex++;
			}

            

        }
		
		public void DrawDebug(RenderTarget rt){
			
			
			foreach(List<Cube> cubeList in cubeLists)			
            	foreach (Cube c in cubeList)
					c.DrawDebug(rt);
				
			foreach(Player p in playerList){
				p.DrawDebug(rt);
				foreach (Cube c in quadTrees[p.Layer].getList(p.Bbox))
					c.DrawDebug(rt, 215);
			}
		}

        public Cube getCollidingCube(FloatRect bbox, int layer)
        {
            foreach (Cube c in quadTrees[layer].getList(bbox))
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
		
		public List<List<Cube>> CubeLists{
			get { return cubeLists; }
		}

    }
}
