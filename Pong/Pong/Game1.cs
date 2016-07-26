using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AuxiliarClasses;

namespace Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D ballTexture;
        Texture2D palletTexture;
        Texture2D easyPressedTexture;
        Texture2D easyTexture;
        Texture2D mediumPressedTexture;
        Texture2D mediumTexture;
        Texture2D hardPressedTexture;
        Texture2D hardTexture;
        Texture2D titleTexture;

        SpriteFont fonte1;

        Objeto ball;
        Objeto pallet1;
        Objeto pallet2;
        Objeto easy;
        Objeto medium;
        Objeto hard;
        Objeto title;

        int placar;
        int lastSecond;
        int palletDelay;

        EGameState _State;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 300;

            Content.RootDirectory = "Content";
            _State = EGameState.ChoosingLevel;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ball = new Objeto();
            ball.SpeedY = 3;
            ball.Posicao = new Rectangle((Window.ClientBounds.Width / 2) - (5), (Window.ClientBounds.Height / 2) - (5), 10, 10);

            pallet1 = new Objeto();
            pallet1.Posicao = new Rectangle((Window.ClientBounds.Width / 2) - 20, Window.ClientBounds.Height - 15, 40, 10);
            pallet1.MaxSpeedX = 8;

            pallet2 = new Objeto();
            pallet2.Posicao = new Rectangle((Window.ClientBounds.Width / 2) - 20, 15, 40, 10);
            pallet2.MaxSpeedX = 8;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ballTexture = Content.Load<Texture2D>("pongball");
            palletTexture = Content.Load<Texture2D>("palheta");
            easyTexture = Content.Load<Texture2D>("easy");
            easyPressedTexture = Content.Load<Texture2D>("easyPressed");
            mediumTexture = Content.Load<Texture2D>("medium");
            mediumPressedTexture = Content.Load<Texture2D>("mediumPressed");
            hardTexture = Content.Load<Texture2D>("hard");
            hardPressedTexture = Content.Load<Texture2D>("hardPressed");
            titleTexture = Content.Load<Texture2D>("title");

            title = new Objeto();
            title.Texture = titleTexture;
            title.Posicao = new Rectangle(0, 10, 300, 120);

            easy = new Objeto();
            easy.Texture = easyTexture;
            easy.Posicao = new Rectangle(75, 150, 150, 60);

            medium = new Objeto();
            medium.Texture = easyTexture;
            medium.Posicao = new Rectangle(75, 230, 150, 60);

            hard = new Objeto();
            hard.Texture = hardTexture;
            hard.Posicao = new Rectangle(75, 310, 150, 60);

            fonte1 = Content.Load<SpriteFont>("Fonte1");

            pallet1.Texture = pallet2.Texture = palletTexture;
            ball.Texture = ballTexture;
            ball.SaveState();

            Window.AllowUserResizing = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            List<Keys> keys = Keyboard.GetState().GetPressedKeys().ToList();

            if (keys.Contains(Keys.Escape))
            {
                _State = EGameState.ChoosingLevel;
                IniciarObjetosMoveis();
                IsMouseVisible = true;
                placar = 0;
            }

            // TODO: Add your update logic here
            if (_State == EGameState.Playing)
            {
                if (keys.Contains(Keys.Right) || keys.Contains(Keys.D))
                {
                    pallet1.AccelerateRight();
                }
                else if (keys.Contains(Keys.Left) || keys.Contains(Keys.A))
                {
                    pallet1.AccelerateLeft();
                }
                else
                    pallet1.Stop();

                if (ball.SpeedY < 0)
                {
                    if (ball.Posicao.Center.X > pallet2.Posicao.Center.X + palletDelay)
                        pallet2.AccelerateRight();
                    else if (ball.Posicao.Center.X < pallet2.Posicao.Center.X - palletDelay)
                        pallet2.AccelerateLeft();
                    else
                        pallet2.Stop();
                }
                else
                    pallet2.Stop();

                if (gameTime.TotalGameTime.Seconds != lastSecond && gameTime.TotalGameTime.Seconds % 10 == 0)
                {
                    if (ball.SpeedY < 0)
                        ball.SpeedY -= 1;
                    else
                        ball.SpeedY += 1;
                }
                lastSecond = gameTime.TotalGameTime.Seconds;

                pallet1.Update();
                ball.Update();
                pallet2.Update();

                ball.CheckInsideWindow(Window, true, ref placar);
                pallet1.CheckInsideWindow(Window);
                pallet2.CheckInsideWindow(Window);

                if (ball.Posicao.Intersects(pallet1.Posicao))
                {
                    if (ball.Posicao.Center.Y > pallet1.Posicao.Center.Y)
                        ball.Posicao.Y = pallet1.Posicao.Bottom;
                    else
                        ball.Posicao.Y = pallet1.Posicao.Top - 10;

                    ball.SpeedY *= -1;
                    ball.SpeedX += pallet1.SpeedX;

                    if (ball.SpeedX > 10)
                        ball.SpeedX = 10;
                    else if (ball.SpeedX < -10)
                        ball.SpeedX = -10;
                }
                else if (ball.Posicao.Intersects(pallet2.Posicao))
                {
                    if (ball.Posicao.Center.Y > pallet2.Posicao.Center.Y)
                        ball.Posicao.Y = pallet2.Posicao.Bottom;
                    else
                        ball.Posicao.Y = pallet2.Posicao.Top - 10;

                    ball.SpeedY *= -1;
                    ball.SpeedX += pallet2.SpeedX;

                    if (ball.SpeedX > 10)
                        ball.SpeedX = 10;
                    else if (ball.SpeedX < -10)
                        ball.SpeedX = -10;
                }
            }
            else if (_State == EGameState.ChoosingLevel)
            { 
                MouseState mouse = Mouse.GetState();

                if (easy.Posicao.Contains(new Point(mouse.X, mouse.Y)))
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        pallet2.MaxSpeedX = 8;
                        palletDelay = 6;
                        _State = EGameState.Playing;
                        IsMouseVisible = false;
                    }
                    else
                        easy.Texture = easyPressedTexture; 
                }
                else
                    easy.Texture = easyTexture;

                if (medium.Posicao.Contains(new Point(mouse.X, mouse.Y)))
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        pallet2.MaxSpeedX = 9;
                        palletDelay = 3;
                        pallet2.Posicao.Width += 10;
                        _State = EGameState.Playing;
                        IsMouseVisible = false;
                    }
                    else
                        medium.Texture = mediumPressedTexture;
                }
                else
                    medium.Texture = mediumTexture;

                if (hard.Posicao.Contains(new Point(mouse.X, mouse.Y)))
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        pallet2.MaxSpeedX = 12;
                        palletDelay = 0;
                        pallet2.Posicao.Width += 20;
                        _State = EGameState.Playing;
                        IsMouseVisible = false;
                    }
                    else
                        hard.Texture = hardPressedTexture;
                }
                else
                    hard.Texture = hardTexture;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (_State == EGameState.Playing)
            {
                ball.Draw(spriteBatch);
                pallet1.Draw(spriteBatch);
                pallet2.Draw(spriteBatch);
                spriteBatch.DrawString(fonte1, "Placar :" + placar, new Vector2(10, 10), Color.Red);
                spriteBatch.DrawString(fonte1, "Velocidade :" + (ball.SpeedY > 0 ? ball.SpeedY : ball.SpeedY*(-1)), new Vector2(10, 25), Color.Red);
            }
            else if (_State == EGameState.ChoosingLevel)
            {
                GraphicsDevice.Clear(Color.White);

                easy.Draw(spriteBatch);
                medium.Draw(spriteBatch);
                hard.Draw(spriteBatch);
                title.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void IniciarObjetosMoveis()
        {
            ball.SpeedY = 3;
            ball.SpeedX = 0;
            ball.Posicao = new Rectangle((Window.ClientBounds.Width / 2) - (5), (Window.ClientBounds.Height / 2) - (5), 10, 10);

            pallet1.Posicao = new Rectangle((Window.ClientBounds.Width / 2) - 20, Window.ClientBounds.Height - 15, 40, 10);
            pallet1.MaxSpeedX = 8;

            pallet2.Posicao = new Rectangle((Window.ClientBounds.Width / 2) - 20, 15, 40, 10);
            pallet2.MaxSpeedX = 8;
        }
    }
}
