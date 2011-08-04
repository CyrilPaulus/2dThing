using System;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using _2dThing;

namespace _2dThing
{
	public class Chat
	{
		private bool writing;
		private Font myFont;
		private String toWrite;
		private Client client;
		private const float messageLifeTime = 5;
		
		List<KeyValuePair<DateTime, String>> msgList;
		
		public Chat (Client client)
		{
			writing = false;
			myFont = new Font("content/arial.ttf");
			toWrite = "";
			msgList = new List<KeyValuePair<DateTime, string>>();
			this.client = client;
		}
		
		public void Draw(RenderTarget rt){
			DateTime now = DateTime.Now;
			
			Text message = new Text("", myFont);
			message.CharacterSize = 14;
			
			int removeCount = 0;
			foreach(KeyValuePair<DateTime, String> pair in msgList){
			 	message.DisplayedString += pair.Value + "\n";
				if((now - pair.Key).TotalSeconds > messageLifeTime)
					removeCount++;
			}
			
			msgList.RemoveRange(0, removeCount);
			message.Position = new Vector2f(14, rt.Height - 54 - message.GetRect().Height);
			rt.Draw(message);
			
			if(writing){
				Text display = new Text("say : " + toWrite + "_", myFont);
				display.CharacterSize = 14;
				display.Position = new Vector2f(14, rt.Height - 36 - display.GetRect().Height);
				rt.Draw(display);
			}
		}
		
		public void update(String c){
			if(c.Equals("\b") && toWrite.Length > 0){				
					toWrite = toWrite.Substring(0, toWrite.Length -1);
			}
			else if (c.Equals("\r") || c.Equals("\n")){
				writing = false;
				sendMessage();
			}
			else
				toWrite += c;
		}
		
		public bool Writing{
			get{ return writing; }
			set { writing = value;}
		}
		
		public void addMsg(String msg){
			msgList.Add(new KeyValuePair<DateTime, String>(DateTime.Now, msg));
		}
		
				
		private void sendMessage(){
			ChatMessage cm = new ChatMessage(client.ClientId);
			cm.Pseudo = client.Pseudo;
			cm.Message = toWrite;
			client.sendPkt(cm, true);			
			toWrite = "";
			
		}
	}
}

