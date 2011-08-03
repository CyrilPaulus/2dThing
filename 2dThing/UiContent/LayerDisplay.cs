using System;
using SFML.Window;
using SFML.Graphics;
using _2dThing.GameContent;

namespace _2dThing
{
	public class LayerDisplay
	{
		private Vector2f position;
		private Sprite layer;
		private ImageManager imageManager;
		private int nbrLayer = World.LAYERNBR;
		
		public LayerDisplay (ImageManager imageManager)
		{
			this.imageManager = imageManager;
			layer = new Sprite(imageManager.GetImage("layer"));
		}
		
		public void Draw(RenderTarget rt, int layerNbr){
			layer.Position = position;
			for(int i = 0; i < nbrLayer; i++){
				
				if(i == layerNbr){
					layer.Position -= new Vector2f(0, 10);
					rt.Draw(layer);
					layer.Position += new Vector2f(0, 10);
				}
				else
					rt.Draw(layer);			
				
				layer.Position += new Vector2f(5, 5);			
				
			}
		}
		
		public Vector2f Position{
			get { return position; }
			set { position = value; }
		}
	}
}

