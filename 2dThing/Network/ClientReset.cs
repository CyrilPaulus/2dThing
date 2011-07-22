using System;

namespace _2dThing
{
	public class ClientReset : Packet
	{
		public ClientReset (int clientId) : base(clientId)			
		{
			this.type = Packet.CLIENTRESET;
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
		}
		
		public new static ClientReset decode(ref Lidgren.Network.NetIncomingMessage msg){
			return new ClientReset(Packet.decode(ref msg).ClientId);
		}
	}
}

