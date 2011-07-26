using System;
using SFML.Window;

namespace _2dThing
{
	public class BlockUpdate : Packet
	{
		Vector2f position;
		bool added;
		byte blockType;
		public BlockUpdate (int clientId) : base(clientId)			
		{
			this.type = Packet.BLOCKUPDATE;
			added = false;
			position = new Vector2f(0,0);
			blockType = 0;
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
			msg.Write(blockType);
		}
		
		public static new BlockUpdate decode (ref Lidgren.Network.NetIncomingMessage msg)
		{
			BlockUpdate bu = new BlockUpdate (Packet.decode (ref msg).ClientId);					
			bu.added = msg.ReadBoolean ();
			bu.position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
			bu.blockType = msg.ReadByte();
			return bu;
		}
		
		public byte BlockType{
			get { return blockType; }
			set { blockType = value;}
		}
		
	}
}

