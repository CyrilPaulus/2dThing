using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SFML.Graphics;
using SFML.Window;

namespace _2dThing
{
	class Program
	{
		static void Main (string[] args)
		{
			bool isClient = false;
			bool isServer = false;
			String ip = "localhost";
			String pseudo = null;
			int index = 0;
			if (args.Length != 0) {
				foreach (string v in args) {
					if (v == "-server") {
						isServer = true;
						isClient = false;
						
					}
					
					if (v == "-client") {
						isServer = false;
						isClient = true;
						ip = args[index +1];						
					}
					
					if( v == "-pseudo"){
						pseudo = args[index + 1];
					}
					
					
					index++;
				}
			}
			
			if (isServer) {
				Server server = new Server ();
				server.run ();
			} else 
			{
				Server server = new Server();
				
				Dictionary<int,Screen> screens = new Dictionary<int, Screen>();
				int screen = 0;
				RenderWindow window = new RenderWindow(new VideoMode(800, 600), "2dThing");				
				Client client = new Client(window, ip);
				screens.Add(Screen.GAME, client);
				
				MainMenu mainMenu = new MainMenu(window, client, server);
				screens.Add(Screen.MAINMENU, mainMenu);
				mainMenu.loadEventHandler();
				
				while(screen >= 0){
					int prevScreen = screen;
					screen = screens[screen].Run();
					
					if(screen >= 0){
						screens[prevScreen].unloadEventHandler();
						screens[screen].loadEventHandler();
					}
				}
				client.Disconnect();
				if(server != null)
					server.stop();
			}
			
			
		}       
	}
}
