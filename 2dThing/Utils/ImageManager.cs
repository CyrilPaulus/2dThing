using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace _2dThing
{
	public class ImageManager
	{
		Dictionary<String, Image> imageSet;
		
		public ImageManager ()			
		{
			imageSet = new Dictionary<string, Image>();
		}
		
		public void Load(String name, String path){
			if(!imageSet.ContainsKey("name")){
				Image myImage = new Image(path);
				imageSet.Add(name, myImage);
			}
		}
		
		
		public Image GetImage(String name){
			if(imageSet.ContainsKey(name))
				return imageSet[name];
			else
			{
				Image myImage = new Image("content/" + name + ".png");
				imageSet.Add(name, myImage);
				return myImage;
			}
				
				
		}
	}
}

