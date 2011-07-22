using System;
using Lidgren.Network;

namespace _2dThing
{
	public class Packet
	{
		public const byte DEFAULT = 0;
		public const byte CLIENTINFO = 1;
		public const byte USERMESSAGE = 2;
		public const byte BLOCKUPDATE =  3;
		public const byte CLIENTDISCONNECT = 4;
		public const byte CLIENTRESET = 5;
		protected byte type;
		protected int clientId;
		
		public Packet (int clientId)
		{
			this.type = 0;
			this.clientId = clientId;
		}
		
		public virtual void encode (ref NetOutgoingMessage msg)
		{
			msg.Write (type);
			msg.Write (clientId);
		}
		
		public static Packet decode (ref NetIncomingMessage msg)
		{
			Packet p = new Packet (0);
			p.type = msg.ReadByte ();
			p.clientId = msg.ReadInt32 ();
			return p;
		}
		
		public int ClientId {
			get { return clientId; }
		}
				
	}
}

