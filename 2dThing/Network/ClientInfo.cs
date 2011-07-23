using System;
using SFML.Graphics;

namespace _2dThing
{
	public class ClientInfo : Packet
	{
		private String pseudo;
		private Color color;

		public ClientInfo (int clientId) : base(clientId)
		{
			type = Packet.CLIENTINFO;
			pseudo = "EMPTY";
			color = new Color(0,0,0);
		}
		
		public String Pseudo {
			get { return pseudo; }
			set { pseudo = value; }
		}
		
		public Color Color {
			get { return color; }
			set { color = value; }
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
			msg.Write (pseudo);
			msg.Write(color.R);
			msg.Write(color.G);
			msg.Write(color.B);
		}
		
		public static new ClientInfo decode (ref Lidgren.Network.NetIncomingMessage msg)
		{
			ClientInfo ci = new ClientInfo (Packet.decode (ref msg).ClientId);					
			ci.Pseudo = msg.ReadString ();
			ci.Color = new Color(msg.ReadByte(), msg.ReadByte(), msg.ReadByte());
			return ci;
		}
	}
}

