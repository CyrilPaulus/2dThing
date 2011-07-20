using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace _2dThing.System
{
    class ImageManager
    {
        private Dictionary<String, Image> set;

        public ImageManager()
        {
            set = new Dictionary<String, Image>();
        }

        public Image Get(String key)
        {
            return set[key];
        }

        public void Load(String key, String path)
        {
            Image img = new Image(path);
            set.Add(key, img);
        }

    }
}
