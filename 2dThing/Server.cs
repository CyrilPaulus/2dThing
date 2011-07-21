using System;
using System.Threading;
using _2dThing.GameContent;
using _2dThing.Utils;
using Lidgren.Network;

namespace _2dThing
{
	public class Server
	{
		Ticker ticker;
		World map;
		DateTime lastTickTime;
		NetServer server;
		
		
		public Server ()
		{
			this.ticker = new Ticker();
			this.map = new World();
			lastTickTime = DateTime.Now;
			NetPeerConfiguration netConfiguration  = new NetPeerConfiguration("2dThing");
			netConfiguration.Port = 55017;
			server = new NetServer(netConfiguration);			
		}
		
		public void run()
		{
			server.Start();
			Console.WriteLine("Server started");
			while(true)
			{
				if(ticker.Tick())
				{
					update((float) (DateTime.Now - lastTickTime).TotalSeconds);
					lastTickTime = DateTime.Now;
				}
				else
				{
					Thread.Sleep(10);
				}
			}
			
		}
		
				
		public void update(float time){
			readIncomingMsg();			
		}
		
		public void readIncomingMsg(){
			NetIncomingMessage msg;
				while ((msg = server.ReadMessage()) != null)
				{
				    switch (msg.MessageType)
				    {
				        case NetIncomingMessageType.VerboseDebugMessage:
				        case NetIncomingMessageType.DebugMessage:
				        case NetIncomingMessageType.WarningMessage:
					    case NetIncomingMessageType.ErrorMessage:
							Console.WriteLine(msg.ReadString());
				            break;
						case NetIncomingMessageType.StatusChanged:
							NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();	
							
							Console.WriteLine(status.ToString() + ": " + msg.ReadString());
							break;
				            
				        default:
				            Console.WriteLine("Unhandled type: " + msg.MessageType);
				            break;
				    }
				    server.Recycle(msg);
				}
		}
		
	}
}

