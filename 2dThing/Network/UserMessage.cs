using System;
using SFML.Window;

namespace _2dThing
{
	public class UserMessage : Packet
	{
		
		//TODO reduce
		DateTime time;
		Input input;
		Vector2f position;
		float ticktime;
		Vector2f eyePosition;
		float fallSpeed;
		byte layer;
		bool noclip;
		
		public UserMessage (int clientId) : base(clientId)
		{
			type = Packet.USERMESSAGE;
			time = DateTime.Now;
			position = new Vector2f (0, 0);
			input = new Input();
			ticktime = 0;
		}
		
		public override void encode (ref Lidgren.Network.NetOutgoingMessage msg)
		{
			base.encode (ref msg);
			msg.Write (time.ToBinary ());
			
			msg.Write (input.Up);
			msg.Write (input.Down);
			msg.Write (input.Left);
			msg.Write (input.Right);
			msg.Write (position.X);
			msg.Write (position.Y);
			msg.Write (ticktime);
			msg.Write(eyePosition.X);
			msg.Write(eyePosition.Y);
			msg.Write(fallSpeed);
			msg.Write(layer);
			msg.Write(noclip);
		}
		
		public new static UserMessage decode (ref Lidgren.Network.NetIncomingMessage msg)
		{
			UserMessage um = new UserMessage (Packet.decode (ref msg).ClientId);
			um.time = DateTime.FromBinary (msg.ReadInt64 ());
			um.input.Up = msg.ReadBoolean ();
			um.input.Down = msg.ReadBoolean ();
			um.input.Left = msg.ReadBoolean ();
			um.input.Right = msg.ReadBoolean ();
			um.position = new Vector2f (msg.ReadFloat (), msg.ReadFloat ());
			um.ticktime = msg.ReadFloat();
			um.EyePosition = new Vector2f(msg.ReadFloat(), msg.ReadFloat());
			um.fallSpeed = msg.ReadFloat();
			um.layer = msg.ReadByte();
			um.noclip = msg.ReadBoolean();
			return um;			
		}
		
		public DateTime Time {
			get { return time; }
			set { time = value; }
		}
		
		public Input Input{
			get { return input; }
			set { input = value; }
		}
		
		public Vector2f Position {
			get { return position; }
			set { position = value; }
		}
		
		public Vector2f EyePosition {
			get { return eyePosition; }
			set { eyePosition = value; }
		}
		
		public float Ticktime {
			get { return ticktime; }
			set { ticktime = value; }
		}
		
		public float FallSpeed {
			get { return fallSpeed; }
			set { fallSpeed = value; }
		}
		
		public byte Layer {
			get { return layer; }
			set { layer = value; }
		}
		
		public bool Nolcip{
			get { return noclip; }
			set { noclip = value; }
		}
		
	}
}

