using System;
using Lidgren.Network;

namespace _2dThing
{
	public class NetworkClient
	{
		DateTime lastHeartBeat;
		
		//Will be used as client ID
		NetConnection connection;
		private int clientId;
		String pseudo;
		
		public NetworkClient (int clientId, NetConnection connection)
		{
			this.connection = connection;
			lastHeartBeat = DateTime.Now;
			pseudo = "Anon " + clientId;
			this.clientId = clientId;
		}
		
		public void HeartBeat ()
		{
			lastHeartBeat = DateTime.Now;
		}
		
		public DateTime LastHeartBeat {
			get { return lastHeartBeat; }
		}
		
		public NetConnection Connection {
			get{ return connection; }
		}
		
		public int ClientId {
			get { return clientId; }
		}
		
		public String Pseudo {
			get { return pseudo; }
			set { pseudo = value; }
		}
	}
}

