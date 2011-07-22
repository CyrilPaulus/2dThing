using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace _2dThing
{
	class Program
	{
		static void Main (string[] args)
		{
			bool isClient = true;
			bool isServer = false;
			
			if (args.Length != 0) {
				foreach (string v in args) {
					if (v == "-server") {
						isServer = true;
						isClient = false;
						break;
					}
					
					if (v == "-client") {
						isServer = false;
						isClient = true;
					}
				}
			}
			
			if (isClient) {
				Client client = new Client ();
				client.run ();
			} else if (isServer) {
				Server server = new Server ();
				server.run ();
			} else {
				//LOCAL GAME				
			}
		}       
	}
}
