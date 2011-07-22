using System;
using SFML.Window;

namespace _2dThing
{
	public class BlockUpdate : Packet
	{
		Vector2f position;
		bool added;
		public BlockUpdate (int clientId) : base(clientId)			
		{
			this.type = Packet.BLOCKUPDATE;
			added = false;
			position = new Vector2f(0,0);
		}
		
		public bool Added{
			get { return added; }
			set { added = value; }
		}
		
		public Vector2f Position{
			get { return position; }
			set { position = value; }
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
			msg.Write(added);
			msg.Write(position.X);
			msg.Write(position.Y);
		}
		
		public static new BlockUpdate decode (ref Lidgren.Network.NetIncomingMessage msg)
		{
			BlockUpdate bu = new BlockUpdate (Packet.decode (ref msg).ClientId);					
			bu.added = msg.ReadBoolean ();
			bu.position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
			return bu;
		}
		
	}
}

