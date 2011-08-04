using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using _2dThing.Utils;
using SFML.Window;
using SFML.Graphics;

namespace _2dThing.GameContent {
	public class World {
		public static int LAYERNBR = 4;
		private List<List<Cube>> cubeLists;
		private List<Player> playerList;
		private float gravity = 600;
		private float maxFallSpeed = 2000;
		private List<QuadTree> quadTrees;
		private ImageManager imageManager;
		
		public World(ImageManager imageManager) {
			this.imageManager = imageManager;
			playerList = new List<Player>();            
			cubeLists = new List<List<Cube>>();
			quadTrees = new List<QuadTree>();
			
			for (int i = 0; i < LAYERNBR; i++) {
				cubeLists.Add(new List<Cube>());
				quadTrees.Add(new QuadTree(10, new Vector2f(90, 90)));
			}
		}
		
		public void Draw(RenderTarget rt, int layer) {
			Vector2f origin = rt.ConvertCoords(0, 0);
			Shape fog = Shape.Rectangle(new FloatRect(origin.X, origin.Y, rt.DefaultView.Size.X, rt.DefaultView.Size.Y), new Color(100, 149, 237, 150));
			
			int layerIndex = 0;
			foreach (List<Cube> cubeList in cubeLists) {
				
				if (layerIndex > layer)
					break;
				
				foreach (Cube c in cubeList)
					c.Draw(rt);
				
				foreach (Player p in playerList)					
					if (p.Layer == layerIndex)
						p.Draw(rt);
						
				if (layerIndex < layer)
					rt.Draw(fog);			
						
				layerIndex++;
			}           

		}
		
		public void DrawUpperLayer(RenderTarget rt, int layer) {
			if (layer + 1 < World.LAYERNBR) {
				foreach (Player p in playerList)
					if (p.Layer == layer + 1)
						p.Draw(rt, new Color(255, 150, 150, 150));
				
				foreach (Cube c in cubeLists[layer + 1])
					c.Draw(rt, new Color(255, 150, 150, 150));
				
			}
		}
		
		public void DrawDebug(RenderTarget rt) {		
			foreach (List<Cube> cubeList in cubeLists)			
				foreach (Cube c in cubeList)
					c.DrawDebug(rt);
				
			foreach (Player p in playerList) {
				p.DrawDebug(rt);
				foreach (Cube c in quadTrees[p.Layer].GetList(p.Bbox))
					c.DrawDebug(rt, 215);
			}
		}
		
		public bool AddCube(Vector2f pos, int type, int layer) {
			Vector2f gridPos = new Vector2f((float)Math.Floor((pos.X / Cube.WIDTH)) * Cube.WIDTH, (float)Math.Floor((pos.Y / Cube.HEIGHT)) * Cube.HEIGHT);
			Cube cube = new Cube(type, imageManager);
			cube.Position = gridPos;
			bool exist = false;

			foreach (Cube c in quadTrees[layer].GetList(cube.Bbox))
				if (c.Bbox.Intersects(cube.Bbox))
					exist = true;

			foreach (Player p in playerList)
				if (p.Bbox.Intersects(cube.Bbox) && p.Layer == layer)
					exist = true;

			if (!exist) {
				cubeLists[layer].Add(cube);
				quadTrees[layer].AddCube(cube);
			}
			return !exist;
		}
		
		public void ForceAddCube(Vector2f pos, int type, int layer) {
			Vector2f gridPos = new Vector2f((float)Math.Floor((pos.X / Cube.WIDTH)) * Cube.WIDTH, (float)Math.Floor((pos.Y / Cube.HEIGHT)) * Cube.HEIGHT);
			Cube cube = new Cube(type, imageManager);
			cube.Position = gridPos;
			cubeLists[layer].Add(cube);
			quadTrees[layer].AddCube(cube);
		}

		public void DeleteCube(Vector2f pos, int layer) {
			foreach (Cube c in cubeLists[layer]) {
				if (c.Bbox.Contains(pos.X, pos.Y)) {
					cubeLists[layer].Remove(c);
					quadTrees[layer].RemoveCube(c);
					return;
				}
			}
		}

		public void AddPlayer(Player p) {
			playerList.Add(p);           
		}

		public void DeletePlayer(Player p) {
			playerList.Remove(p);
		}		

		public Cube GetCollidingCube(FloatRect bbox, int layer) {
			foreach (Cube c in quadTrees[layer].GetList(bbox)) {
				if (c.Bbox.Intersects(bbox))
					return c;
			}
			return null;
		}	
		
		public List<List<Cube>> CubeLists {
			get { return cubeLists; }
		}
		
		public float Gravity {
			get { return gravity; }
			set { gravity = value; }
		}
		
		public float MaxFallSpeed {
			get{ return maxFallSpeed;}
			set{ maxFallSpeed = value;}
		}
		
		public List<Player> Players {
			get { return playerList; }
		}
	}
}
