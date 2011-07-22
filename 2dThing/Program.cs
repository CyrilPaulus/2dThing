using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SFML.Graphics;

namespace _2dThing
{
	class Program
	{
		static void Main (string[] args)
		{
			bool isClient = false;
			bool isServer = false;
			String ip = "localhost";
			int index = 0;
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
						ip = args[index +1];
						break;
					}
					index++;
				}
			}
			
			if (isClient) {
				Client client = new Client (ip);
				client.run ();
			} else if (isServer) {
				Server server = new Server ();
				server.run ();
			} else {				
				Server server = new Server ();
				Thread serverThread = new Thread(server.run);
				serverThread.Start();
				
				Client client = new Client();
				client.run();
				server.stop();
			}
		}       
	}
}
