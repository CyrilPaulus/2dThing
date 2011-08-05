using System;

namespace _2dThing {
	public class WorldReset : Packet {
		
		public WorldReset(int clientId) : base(clientId) {
			this.type = Packet.WORLDRESET;
		}		
		
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
		}
		
		public new static WorldReset decode(ref Lidgren.Network.NetIncomingMessage msg){
			return new WorldReset(Packet.decode(ref msg).ClientId);
		}
	}
}

