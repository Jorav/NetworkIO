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
        public Camera Camera { get; private set; }

        public static int ScreenHeight;
        public static int ScreenWidth;

        Sprite background;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            //_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Width;
            //_graphics.IsFullScreen = true; //TODO: Add support for full screen without stretching
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;
            Camera = new Camera();

            _graphics.ApplyChanges();
            Texture2D textureHull = Content.Load<Texture2D>("parts/Hull_12");
            Texture2D textureHullRotating = Content.Load<Texture2D>("parts/Hull_rotating");
            Texture2D textureSprayGun = Content.Load<Texture2D>("parts/SprayGun");
            Texture2D projectile = Content.Load<Texture2D>("projectiles/SprayBullet");
            Texture2D background = Content.Load<Texture2D>("background");
            this.background = new Sprite(background) { Position = new Vector2(ScreenWidth / 2, ScreenHeight / 2) };

            //TODO: add factories to replace this bloated code
            Composite e = new Composite(new Sprite(textureHullRotating), new Vector2(50, 50), 0, 1, 1.3f, 100f,
                        true, true, 0.1f, 0.05f, 30f);
            controllers = new List<Controller>()
            {
                new Player(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureSprayGun), new Vector2(50,50), 0, 1, 1.3f, 100f, 50f, 1f,
                            new Projectile(new Sprite(projectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(100,100), 0, 1, 1.3f, 100f, 50f, 1f,
                            new Projectile(new Sprite(projectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                        //e
                    }, Camera
                )
            };

            //projectile.mass 0.05, friction 0.05 great for a "liquid spray"
            e.AddEntity(new Shooter(new Sprite(textureSprayGun), new Vector2(70, 70), 0, 1, 1.3f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(projectile), new Vector2(50, 50), 0, 0.1f, 0, 100f, 3, 1, false, false, 0.01f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f), 0);
            

           controllers.Add(new ChaserAI(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(500,500), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(projectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(700,700), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(projectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                    }, controllers[0])
            );
            controllers.Add(new Controller( //att lägga till en till fick det att explodera typ... weird. och de trycker inte bort varandra
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(600,600), 0, 1, 1f, 1000f, 50f, 10,
                            new Projectile(new Sprite(projectile), new Vector2(50,50), 0, 1, 0, 100f, 3, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(650,650), 0, 1, 1f, 1000f, 50f, 10,
                            new Projectile(new Sprite(projectile), new Vector2(50,50), 0, 1, 0, 100f, 3, 1),
                        true, true, 0.1f, 0.05f, 30f)
                    }/*, controllers[0]*/)
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
            //background.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: Camera.Transform);
            //Vector2 CameraPosition = controllers[0].Position - new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.5f);
           // Matrix m = Matrix.CreateTranslation(new Vector3(-CameraPosition, 0));
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, m);
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, Matrix.CreateScale(0.5f));
            foreach (Controller c in controllers)
                c.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
