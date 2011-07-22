using System;

namespace _2dThing
{
	public class ClientDisconnect : Packet
	{
		public ClientDisconnect (int clientId) : base(clientId)			
		{
			this.type = Packet.CLIENTDISCONNECT;
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
		}
		
		public new static ClientDisconnect decode(ref Lidgren.Network.NetIncomingMessage msg){
			return new ClientDisconnect(Packet.decode(ref msg).ClientId);
		}
	}
}

