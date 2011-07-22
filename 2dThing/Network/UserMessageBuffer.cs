using System;
using _2dThing.GameContent;
using _2dThing.Utils;

namespace _2dThing
{
	public class UserMessageBuffer
	{
		const int capacity = 1000;
		UserMessage[] buffer;
		int head;
		int tail;
		
		public UserMessageBuffer ()
		{
			buffer = new UserMessage[capacity];
			head = 0;
			tail = 0;
		}
		
		public void insert(UserMessage um){
			buffer[head] = um;
			advance();
		}
		
		public void advance(){
			tail = (tail + 1) % capacity;
		}
		
		public void clientCorrection(Player p, UserMessage um){
			while(um.Time > buffer[head].Time && head != tail){
				advance();
			}
			
			
			if(head != tail && um.Time.Equals(buffer[head].Time)){
				
				if(VectorUtils.Distance(buffer[head].Position, um.Position) > 1){
				
					DateTime currentTime = um.Time;
					Input currentInput = um.Input;
					p.Position = um.Position;
					
					advance();
					int index = head;
					
					while(index != tail){
						float deltaTime = (float) (buffer[index].Time - currentTime).TotalSeconds;
																    
						p.update(deltaTime, currentInput);
						
						currentTime = buffer[index].Time;
						currentInput = buffer[index].Input;
						
						buffer[index].Position = p.Position;
						advance();				
						
					}
				}
			}
		}
		
	}
}

