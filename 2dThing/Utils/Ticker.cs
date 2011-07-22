using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2dThing.Utils
{
	class Ticker
	{
		float ticktime;
		DateTime lastTick;

		public Ticker ()
		{
			this.ticktime = 20;
			this.lastTick = DateTime.Now;

		}

		public Ticker (float ticktime)
		{
			this.ticktime = ticktime;
			this.lastTick = DateTime.Now;
		}

		public bool Tick ()
		{
			DateTime now = DateTime.Now;
			if ((now - lastTick).TotalMilliseconds > ticktime) {
				lastTick = now;
				return true;
			}
			return false;
		}

		public float Ticktime {
			get { return ticktime; }
			set { ticktime = value; }
		}
       
	}
}
