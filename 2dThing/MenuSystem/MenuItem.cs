using System;
using SFML.Window;
using SFML.Graphics;

namespace _2dThing
{
	public class MenuItem
	{
		Font textFont;
		Vector2f position;
		String item;
		Action action;
		Text itemText;
		
		public MenuItem (String item, Vector2f position, Action action)
		{			
			this.item = item;
			this.position = position;
			this.action = action;
			this.textFont = new Font("content/arial.ttf");
			this.itemText = new Text(item, textFont);
			this.itemText.Position = position;
		}
		
		public Font TextFont{
			set { textFont = value; itemText.Font = textFont; }
		}
		
		public void doAction(){
			action();
		}
		
		public void Draw(RenderTarget rt){
			rt.Draw(itemText);
		}
	}
}

