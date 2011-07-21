﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using _2dThing.System;
using _2dThing.System.GameContent;

namespace _2dThing
{
    class Game
    {
        RenderWindow window;
        RenderImage world;
        RenderImage ui;

        Ticker ticker;

        Sprite mouse;

        World map;

        //Dumb stuff to delete      
        Player player;

        public Game()
        {
            window = new RenderWindow(new VideoMode(800, 600), "2dThing is back bitches");
            world = new RenderImage(800, 600);
            ui = new RenderImage(800, 600);
            map = new World();
            ticker = new Ticker();
            ticker.TPS = 60;

            window.Closed += new EventHandler(OnClose);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);
            window.ShowMouseCursor(false);
            window.SetFramerateLimit(60);
            mouse = new Sprite(new Image("content/mouse.png"));

            player = new Player(map);
            map.addCube(new Vector2f(0,80));
            map.addPlayer(player);
            world.DefaultView.Center = new Vector2f(0, 0);
            world.SetView(world.DefaultView);
        }

        public void run()
        {
            //Dumb stuff to remove
            Font myFont = new Font("content/arial.ttf");            
            Text text = new Text("Fps:", myFont);
            text.Position = new Vector2f(0, 0);
            text.CharacterSize = 20;
            text.Color = Color.Black;
            DateTime lastTickTime = DateTime.Now;
            while (window.IsOpened())
            {
                if (window.GetFrameTime() != 0)
                {
                    text.DisplayedString = "Fps: " + (int)(1f / window.GetFrameTime() * 1000);
                }
                window.DispatchEvents();

                if (ticker.Tick())
                {
                    update((float) (DateTime.Now - lastTickTime).TotalSeconds);
                    lastTickTime = DateTime.Now;
                }

                world.Clear(new Color(100, 149, 237));
                map.Draw(world);                
                world.Display();

                ui.Clear(new Color(255, 255, 255, 0));
                ui.Draw(mouse);
                ui.Draw(text);
                ui.Display();

                window.Clear(new Color(100, 149, 237));                
                window.Draw(new Sprite(world.Image));
                window.Draw(new Sprite(ui.Image));
                window.Display();                
            }
        }

        /// <summary>
        /// Update the game state
        /// </summary>
        /// <param name="frameTime">time of last frame in seconds</param>
        private void update(float frameTime)
        {
            player.Left = Keyboard.IsKeyPressed(Keyboard.Key.Left);
            player.Right = Keyboard.IsKeyPressed(Keyboard.Key.Right);
            player.Up = Keyboard.IsKeyPressed(Keyboard.Key.Up);
            player.Down = Keyboard.IsKeyPressed(Keyboard.Key.Down);
            player.lookAt(getWorldMouse());
            player.update(frameTime);
            updateCam();
        }

        //Events
        static void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        void OnMouseMoved(object sender, EventArgs e)
        {
            MouseMoveEventArgs a = (MouseMoveEventArgs)e;
            mouse.Position = new Vector2f(a.X, a.Y);            
        }

        void OnMouseButtonPressed(object sender, EventArgs e)
        {
            MouseButtonEventArgs a = (MouseButtonEventArgs)e;
            switch (a.Button)
            {
                case Mouse.Button.Left:
                    map.addCube(getWorldMouse());
                    break;
                case Mouse.Button.Right:
                    map.deleteCube(getWorldMouse());
                    break;
                default:
                    break;
            }
        }

        void OnKeyPressed(object sender, EventArgs e)
        {
            KeyEventArgs a = (KeyEventArgs)e;
            switch (a.Code)
            {
                case Keyboard.Key.Left:
                    //world.DefaultView.Move(new Vector2f(-10, 0));                    
                    world.SetView(world.DefaultView);                   
                    break;
                case Keyboard.Key.Right:
                    //world.DefaultView.Move(new Vector2f(10, 0));
                    world.SetView(world.DefaultView);
                    break;
                case Keyboard.Key.Up:
                    //world.DefaultView.Move(new Vector2f(0, -10));
                    world.SetView(world.DefaultView);
                    break;
                case Keyboard.Key.Down:
                    //world.DefaultView.Move(new Vector2f(0, 10));
                    world.SetView(world.DefaultView);
                    break;
                case Keyboard.Key.R:
                    player.Position = new Vector2f(0, 0);
                    break;
                default:
                    break;
            }
        }

        private Vector2f getWorldMouse()
        {
            return world.ConvertCoords((uint)mouse.Position.X, (uint)mouse.Position.Y);
        }

        public void updateCam()
        {
            
           
            float left = world.DefaultView.Center.X - world.DefaultView.Size.X / 2;
            float right = world.DefaultView.Center.X + world.DefaultView.Size.X / 2;
            if (player.Bbox.Left - 100 < left)
                world.DefaultView.Move(new Vector2f(player.Bbox.Left - 100 - left, 0));
            else if (player.Bbox.Left + player.Bbox.Width + 100 > right)
                world.DefaultView.Move(new Vector2f(player.Bbox.Left + player.Bbox.Width + 100 - right, 0));


            float top = world.DefaultView.Center.Y - world.DefaultView.Size.Y / 2;
            float bottom = world.DefaultView.Center.Y + world.DefaultView.Size.Y / 2;

            if (player.Bbox.Top - 100 < top)
                world.DefaultView.Move(new Vector2f(0, player.Bbox.Top - 100 - top));
            else if (player.Bbox.Top + player.Bbox.Height + 100 > bottom)
                world.DefaultView.Move(new Vector2f(0, player.Bbox.Top + player.Bbox.Height + 100 - bottom));

            world.SetView(world.DefaultView);
        }
    }
}
