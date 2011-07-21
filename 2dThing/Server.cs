using System;
using System.Threading;
using _2dThing.GameContent;
using _2dThing.Utils;

namespace _2dThing
{
	public class Server
	{
		Ticker ticker;
		World map;
		DateTime lastTickTime;
		
		public Server ()
		{
			this.ticker = new Ticker();
			this.map = new World();
			lastTickTime = DateTime.Now;
		}
		
		public void run()
		{
			while(true){
				if(ticker.Tick()){
					update((float) (DateTime.Now - lastTickTime).TotalSeconds);
					lastTickTime = DateTime.Now;
				}
				else{
				Thread.Sleep(10);
				}
			}
			
		}
		
		public void update(float time){
		}
		
		
	}
}

