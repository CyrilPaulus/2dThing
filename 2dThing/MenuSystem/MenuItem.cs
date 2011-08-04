using System;
using SFML.Window;
using SFML.Graphics;

namespace _2dThing
{
	public class MenuItem
	{
		Font textFont;
		Vector2f position;
		Func<int> action;		
		Text itemText;
		
		public MenuItem (String item, Vector2f position, Func<int> action)
		{			
			this.position = position;
			this.action = action;
			this.textFont = new Font("content/arial.ttf");
			this.itemText = new Text(item, textFont);
			this.itemText.Position = position;
		}
		
		public Font TextFont{
			set { textFont = value; itemText.Font = textFont; }
		}
		
		public int doAction(){
			return action();
		}
		
		public void Draw(RenderTarget rt, bool selected){
			if(selected)
				itemText.Color = new Color(255, 201, 14);
			else
				itemText.Color = new Color(255, 255, 255);
			rt.Draw(itemText);
		}
		
		public void CenterX(int width){
			position = new Vector2f((width - itemText.GetRect().Width) / 2, position.Y);
			itemText.Position = position;
		}
		
		public FloatRect Bbox{
			get { return itemText.GetRect();}
		}
		
		public String Item{
			set { itemText.DisplayedString = value; }
			get { return itemText.DisplayedString; }
		}
	}
}

