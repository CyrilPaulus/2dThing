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
			
			ImageManager imageManager = new ImageManager();
			
			if (isServer) {
				Server server = new Server(imageManager);
				server.run ();
			} else 
			{
				RenderWindow window = new RenderWindow(new VideoMode(800, 600), "2dThing");	
								
				
				Dictionary<int,Screen> screens = new Dictionary<int, Screen>();
				int screen = 0;
				
				Server server = new Server(imageManager);
				Client client = new Client(window, imageManager);
				client.IP = ip;
				
				MainMenu mainMenu = new MainMenu(window, imageManager, client, server);
				ConnectMenu connectMenu = new ConnectMenu(window, imageManager, client, server);
							
				screens.Add(Screen.MAINMENU, mainMenu);
				screens.Add(Screen.GAME, client);
				screens.Add(Screen.CONNECT, connectMenu);
				
				if(isClient){
					client.Connect();
					client.LoadEventHandler();
					screen = Screen.GAME;
				}
				else
					mainMenu.LoadEventHandler();
				
				while(screen >= 0){
					int prevScreen = screen;
					screen = screens[screen].Run();
					
					if(screen >= 0){
						screens[prevScreen].UnloadEventHandler();
						screens[screen].LoadEventHandler();
					}
				}
				client.Disconnect();
				if(server != null)
					server.stop();
			}
			
			
		}       
	}
}
