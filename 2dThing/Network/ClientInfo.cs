using System;

namespace _2dThing
{
	public class ClientInfo : Packet
	{
		private String pseudo;

		public ClientInfo (int clientId) : base(clientId)
		{
			type = Packet.CLIENTINFO;
		}
		
		public String Pseudo {
			get { return pseudo; }
			set { pseudo = value; }
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
			msg.Write (pseudo);
		}
		
		public static new ClientInfo decode (ref Lidgren.Network.NetIncomingMessage msg)
		{
			ClientInfo ci = new ClientInfo (Packet.decode (ref msg).ClientId);					
			ci.Pseudo = msg.ReadString ();
			return ci;
		}
	}
}

