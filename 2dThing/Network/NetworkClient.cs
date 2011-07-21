using System;
using Lidgren.Network;

namespace _2dThing
{
	public class NetworkClient
	{
		DateTime lastHeartBeat;
		
		//Will be used as client ID
		NetConnection connection;
		
		String pseudo;
		
		public NetworkClient (NetConnection connection)
		{
			this.connection = connection;
			lastHeartBeat = DateTime.Now;
			pseudo = connection.RemoteEndpoint.ToString();
		}
		
		public void HeartBeat(){
			lastHeartBeat = DateTime.Now;
		}
		
		public DateTime LastHeartBeat{
			get { return lastHeartBeat; }
		}
		
		public NetConnection Connection{
			get{ return connection; }
		}
		
		public String Pseudo
		{
			get { return pseudo; }
			set { pseudo = value; }
		}
	}
}

