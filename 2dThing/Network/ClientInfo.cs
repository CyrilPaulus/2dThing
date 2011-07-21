using System;

namespace _2dThing
{
	public class ClientInfo : Packet
	{
		private String pseudo;
		public ClientInfo () : base()
		{
			type = Packet.CLIENTINFO;
		}
		
		public String Pseudo
		{
			get { return pseudo; }
			set { pseudo = value; }
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
			msg.Write(pseudo);
		}
		
		public static new ClientInfo decode(ref Lidgren.Network.NetIncomingMessage msg)
		{
			Packet.decode(ref msg);
			ClientInfo ci = new ClientInfo();
			ci.Pseudo = msg.ReadString();
			return ci;
		}
	}
}

