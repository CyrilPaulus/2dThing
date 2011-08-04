using System;
using SFML.Window;
using SFML.Graphics;

namespace _2dThing {
	public class MenuItem {
		private Font textFont;
		private Vector2f position;
		private Func<int> action;
		private Text itemText;
		
		public MenuItem(String item, Vector2f position, Func<int> action) {			
			this.position = position;
			this.action = action;
			this.textFont = new Font("content/arial.ttf");
			this.itemText = new Text(item, textFont);
			this.itemText.Position = position;
		}		
		
		public int DoAction() {
			return action();
		}
		
		public void Draw(RenderTarget rt, bool selected) {
			if (selected)
				itemText.Color = new Color(255, 201, 14);
			else
				itemText.Color = new Color(255, 255, 255);
			rt.Draw(itemText);
		}		
		
		public FloatRect Bbox {
			get { return itemText.GetRect();}
		}
		
		public void CenterX(int width) {
			position = new Vector2f((width - itemText.GetRect().Width) / 2, position.Y);
			itemText.Position = position;
		}
		
		public String Item {
			set { itemText.DisplayedString = value; }
			get { return itemText.DisplayedString; }
		}
		
		public Font TextFont {
			set { textFont = value;
				itemText.Font = textFont; }
		}
	}
}

