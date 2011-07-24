using System;

namespace _2dThing
{
	public class ChatMessage : Packet
	{
		private String message;
		private String pseudo;

		public ChatMessage (int clientId) : base(clientId)
		{
			type = Packet.CHATMESSAGE;
			this.pseudo = "";
			this.message = "";
		}
		
		public String Pseudo {
			get { return pseudo; }
			set { pseudo = value; }
		}
		
		public String Message {
			get { return message; }
			set { message = value; }
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
			msg.Write (pseudo);
			msg.Write( message);
		}
		
		public static new ChatMessage decode (ref Lidgren.Network.NetIncomingMessage msg)
		{
			ChatMessage cm = new ChatMessage (Packet.decode (ref msg).ClientId);					
			cm.Pseudo = msg.ReadString ();
			cm.Message = msg.ReadString ();
			return cm;
		}
	}
}
