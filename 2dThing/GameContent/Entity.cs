﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace _2dThing.GameContent {
	public class Entity {
		protected Sprite sprite;
		protected Vector2f offset;
		protected Vector2f position;
		protected Vector2f size;

		public Entity(ImageManager imageManager) {            
			Sprite = new Sprite(imageManager.GetImage("cube"));
			offset = new Vector2f(0, 0);
			position = new Vector2f(0, 0);			
		}
		
		public virtual void Draw(RenderTarget world) {
			sprite.Position = Position + offset;
			world.Draw(sprite);
          	         
		}
		
		public virtual void Draw(RenderTarget world, Color color) {
			Color original = sprite.Color;
			sprite.Color = color;
			Draw(world);
			sprite.Color = original;
		}
		
		public virtual void DrawDebug(RenderTarget world) {
			Shape rectangle = Shape.Rectangle(Bbox, new Color(237, 28, 36, 100), -2, new Color(255, 201, 14));
			world.Draw(rectangle); 
		}
		
		public virtual void DrawDebug(RenderTarget world, byte alpha) {
			Shape rectangle = Shape.Rectangle(Bbox, new Color(237, 28, 36, alpha), -2, new Color(255, 201, 14));
			world.Draw(rectangle); 
		}

		public virtual void DrawUI(RenderTarget ui) {
           
		}

		public virtual void Update(float frameTime) {

		}
		
		public virtual FloatRect Bbox {
			get { return new FloatRect(Position.X, Position.Y, size.X, size.Y); }
		}
		
		public Image Image {
			set { sprite.Image = value;}
		}
		
		public Vector2f Position {
			get { return position; }
			set { position = value; }
		}

		public Vector2f Scale {
			get { return sprite.Scale; }
			set { sprite.Scale = value; }
		}
		
		public Sprite Sprite {
			set {
				sprite = value;
				size = new Vector2f(sprite.Width, sprite.Height);
			}
			get { return sprite;}
		}
    
	}
}
