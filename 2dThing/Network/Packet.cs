using System;
using Lidgren.Network;

namespace _2dThing
{
	public class Packet
	{
		public const byte DEFAULT = 0;
		public const byte CLIENTINFO = 1;
		protected byte type;
		
		public Packet ()
		{
			this.type = 0;
		}
		
		public virtual void encode(ref NetOutgoingMessage msg){
			msg.Write(type);
		}
		
		public static Packet decode(ref NetIncomingMessage msg){
			Packet p = new Packet();
			p.type = msg.ReadByte();
			return p;
		}
				
	}
}

