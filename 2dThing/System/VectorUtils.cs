using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

namespace _2dThing.System.System
{
    class VectorUtils
    {
        public static float Distance(Vector2f ori, Vector2f dest)
        {
            Vector2f diff = dest - ori;
            return (float) Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
        }

        public static Vector2f Normalize(Vector2f ori)
        {
            float dist = Distance(new Vector2f(0, 0), ori);
            return new Vector2f(ori.X / dist, ori.Y / dist);
        }
    }
}
