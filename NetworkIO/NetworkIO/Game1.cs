using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src;
using NetworkIO.src.entities;
using System.Collections.Generic;

namespace NetworkIO
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<Controller> controllers;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //_graphics.IsFullScreen = true; //TODO: Add support for full screen without stretching
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D textureHull = Content.Load<Texture2D>("parts/SprayGun");
            Texture2D projectile = Content.Load<Texture2D>("projectiles/SprayBullet");

            controllers = new List<Controller>()
            {
                new Player(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHull), new Vector2(50,50), 0, 1, 1, 50f, 10,
                            new Projectile(new Sprite(projectile, 0.5f), new Vector2(50,50), 0, 1, 0, 0, 3),
                        true, 0.1f),
                        new Shooter(new Sprite(textureHull), new Vector2(100,100), 0, 1, 1, 50f, 10,
                            new Projectile(new Sprite(projectile, 0.5f), new Vector2(100,100), 0, 1, 0, 0, 3),
                        true, 0.1f)
                    })
            };

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            foreach(Controller c in controllers)
                c.Update(gameTime);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (Controller c in controllers)
                c.Draw(_spriteBatch);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
