using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src;
using NetworkIO.src.controllers;
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D textureHull = Content.Load<Texture2D>("parts/Hull_12");
            Texture2D textureHullRotating = Content.Load<Texture2D>("parts/Hull_rotating");
            Texture2D textureSprayGun = Content.Load<Texture2D>("parts/SprayGun");
            Texture2D projectile = Content.Load<Texture2D>("projectiles/SprayBullet");

            //TODO: add factories to replace this bloated code
            controllers = new List<Controller>()
            {
                new Player(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(50,50), 0, 1, 1.3f, 100f, 50f, 3f,
                            new Projectile(new Sprite(projectile, 0.3f), new Vector2(50,50), 0, 0.3f, 0, 100f, 3, false, false, 0.001f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(100,100), 0, 1, 1.3f, 100f, 50f, 3f,
                            new Projectile(new Sprite(projectile, 0.3f), new Vector2(50,50), 0, 0.3f, 0, 100f, 3, false, false, 0.001f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(200,200), 0, 1, 1.3f, 100f, 50f, 3f,
                            new Projectile(new Sprite(projectile, 0.3f), new Vector2(50,50), 0, 0.3f, 0, 100f, 3, false, false, 0.001f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                    })
            };
           /* controllers.Add(new ChaserAI(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(400,400), 0, 1, 1f, 50f, 10,
                            new Projectile(new Sprite(projectile, 0.5f), new Vector2(50,50), 0, 1, 0, 3),
                        true, true, 0.1f, 0.05f, 30f)
                    }, controllers[0])
            );
            controllers.Add(new ChaserAI( //att lägga till en till fick det att explodera typ... weird. och de trycker inte bort varandra
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(300,300), 0, 1, 1f, 50f, 10,
                            new Projectile(new Sprite(projectile, 0.5f), new Vector2(50,50), 0, 1, 0, 3),
                        true, true, 0.1f, 0.05f, 30f)
                    }, controllers[0])
            );*/
            controllers.Add(new ChaserAI( //att lägga till en till fick det att explodera typ... weird. och de trycker inte bort varandra
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(600,600), 0, 1, 1f, 100f, 50f, 10,
                            new Projectile(new Sprite(projectile, 0.5f), new Vector2(50,50), 0, 1, 0, 100f, 3),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(650,650), 0, 1, 1f, 100f, 50f, 10,
                            new Projectile(new Sprite(projectile, 0.5f), new Vector2(50,50), 0, 1, 0, 100f, 3),
                        true, true, 0.1f, 0.05f, 30f)
                    }, controllers[0])
            );
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            foreach(Controller c in controllers)
                c.Update(gameTime);
            foreach (Controller c1 in controllers)
                foreach (Controller c2 in controllers)
                    if(c1!=c2)
                        c1.Collide(c2);

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
