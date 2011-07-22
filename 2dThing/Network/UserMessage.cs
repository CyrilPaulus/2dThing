using System;
using SFML.Window;

namespace _2dThing
{
	public class UserMessage : Packet
	{
		DateTime time;
		bool up;
		bool down;
		bool left;
		bool right;
		Vector2f position;
		
		public UserMessage (int clientId) : base(clientId)
		{
			type = Packet.USERMESSAGE;
			time = DateTime.Now;
			position = new Vector2f(0,0);
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
			msg.Write(time.ToBinary());
			
			msg.Write(up);
			msg.Write(down);
			msg.Write(left);
			msg.Write(right);
			msg.Write(position.X);
			msg.Write(position.Y);				
		}
		
		public new static UserMessage decode (ref Lidgren.Network.NetIncomingMessage msg)
		{
			UserMessage um = new UserMessage(Packet.decode(ref msg).ClientId);
			um.time = DateTime.FromBinary(msg.ReadInt64());
			um.up = msg.ReadBoolean();
			um.down = msg.ReadBoolean();
			um.left = msg.ReadBoolean();
			um.right = msg.ReadBoolean();
			um.position = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
			return um;			
		}
		
		public DateTime Time{
			get { return time; }
			set {time = value; }
		}
		
		public bool Left{
			get { return left; }
			set { left = value; }
		}
		
		public bool Right{
			get { return right; }
			set { right = value; }
		}
		
		public bool Up{
			get { return up; }
			set { up = value; }
		}
		
		public bool Down{
			get { return down; }
			set { down = value; }
		}
		
		public Vector2f Position{
			get { return position; }
			set { position = value; }
		}
		
		
	}
}

