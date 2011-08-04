using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;
using _2dThing.GameContent;

namespace _2dThing.Utils {
	class QuadTree {
		private int capacity;
		private Vector2f minSize;
		private QuadTreeNode root;

		public QuadTree(int capacity, Vector2f minSize) {
			this.capacity = capacity;
			this.minSize = minSize;
			this.root = new QuadTreeNode(new FloatRect(-1000000, -1000000, 2000000, 2000000), capacity, minSize);
		}

		public void AddCube(Cube b) {
			root.AddCube(b);
		}

		public void RemoveCube(Cube b) {
			root.Remove(b);
		}

		public List<Cube> GetList(FloatRect bbox) {
			return root.GetList(bbox);
		}
		
	}

	class QuadTreeNode {
		private FloatRect range;
		private List<Cube> cubeList;
		private List<QuadTreeNode> child;
		private int capacity;
		private Vector2f minSize;

		public QuadTreeNode(FloatRect range, int capacity, Vector2f minSize) {
			this.range = range;
			this.capacity = capacity;
			this.child = null;
			this.cubeList = new List<Cube>();
			this.minSize = minSize;
		}

		public void AddCube(Cube b) {
			if (child != null) {
				foreach (QuadTreeNode c in child) {
					if (c.range.Intersects(b.Bbox)) {
						c.AddCube(b);
					}
				}
			} else if (cubeList.Count < capacity) {
				cubeList.Add(b);
			} else {
				this.Explode();
				this.AddCube(b);
			}
		}

		public void Explode() {
			if (range.Width <= minSize.X || range.Height <= minSize.Y)
				return;

			child = new List<QuadTreeNode>();
			float width = range.Width / 2;
			float height = range.Height / 2;
			child.Add(new QuadTreeNode(new FloatRect(range.Left, range.Top, width, height), capacity, minSize));
			child.Add(new QuadTreeNode(new FloatRect(range.Left + width, range.Top, width, height), capacity, minSize));
			child.Add(new QuadTreeNode(new FloatRect(range.Left, range.Top + height, width, height), capacity, minSize));
			child.Add(new QuadTreeNode(new FloatRect(range.Left + width, range.Top + height, width, height), capacity, minSize));
			foreach (Cube b in cubeList) {
				this.AddCube(b);
			}
			cubeList = null;
		}

		public void Remove(Cube b) {
			if (child != null) {
				foreach (QuadTreeNode c in child) {
					if (c.range.Intersects(b.Bbox))
						c.Remove(b);
				}
			} else
				cubeList.Remove(b);
		}

		public List<Cube> GetList(FloatRect bbox) {
			if (child != null) {
				List<Cube > ret = new List<Cube>();
				foreach (QuadTreeNode c in child) {
					if (c.range.Intersects(bbox)) {
						ret = ret.Concat<Cube>(c.GetList(bbox)).ToList();

					}
				}
				return ret;
			} else
				return cubeList;

		}

	}

}
