using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src;
using NetworkIO.src.controllers;
using NetworkIO.src.entities;
using System;
using System.Collections.Generic;

namespace NetworkIO
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private List<Controller> controllers;
        private List<Background> backgrounds;
        public Camera Camera { get; private set; }

        public static int ScreenHeight;
        public static int ScreenWidth;

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
            _graphics.ApplyChanges();

            Texture2D textureHull = Content.Load<Texture2D>("parts/Hull_12");
            Texture2D textureHullRotating = Content.Load<Texture2D>("parts/Hull_rotating");
            Texture2D textureSprayGun = Content.Load<Texture2D>("parts/SprayGun");
            Texture2D textureProjectileSpray = Content.Load<Texture2D>("projectiles/SprayBullet");
            Texture2D textureProjectile = Content.Load<Texture2D>("projectiles/Bullet");
            Texture2D textureSolar = Content.Load<Texture2D>("background/solar");
            Texture2D texturecloudCreepy = Content.Load<Texture2D>("background/cloud_creepy");
            Texture2D textureCloudCreepyBlurry = Content.Load<Texture2D>("background/cloud_creepy_blurry");

            //TODO: add factories to replace this bloated code
            Composite e = new Composite(new Sprite(textureHullRotating), new Vector2(50, 50), 0, 1, 1.3f, 100f,
                        true, true, 0.1f, 0.05f, 30f);
            controllers = new List<Controller>()
            {
                new Player(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureSprayGun), new Vector2(0,0), 0, 1, 1.3f, 100f, 50f, 1f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(0,0), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(0,100), 0, 1, 1.3f, 100f, 50f, 1f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(0,100), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                        //e
                    })
            };
            Camera = ((Player)controllers[0]).Camera;

            //projectile.mass 0.05, friction 0.05 great for a "liquid spray"
            e.AddEntity(new Shooter(new Sprite(textureSprayGun), new Vector2(70, 70), 0, 1, 1.3f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50, 50), 0, 0.1f, 0, 100f, 3, 1, false, false, 0.01f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f), 0);


            controllers.Add(new ChaserAI(
                     new List<Entity>()
                     {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(-100,-100), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(-200,-200), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(-300,-300), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                     }, controllers[0])
             );
            controllers.Add(new IndecisiveAI(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(467,213), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(212,512), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(512,413), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                    })
            );
            controllers.Add(new RandomAI(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(886,1243), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(241,253), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(354,-3), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                    })
            );
            controllers.Add(new CircularAI(
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(200,500), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(600,600), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(300,300), 0, 1, 1.0f, 100f, 10f, 1.5f,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 0.04f, 0, 100f, 300, 1, false, false, 0.03f, 1, 1),
                        true, true, 0.1f, 0.05f, 30f)
                    })
            );
            controllers.Add(new Controller( //att lägga till en till fick det att explodera typ... weird. och de trycker inte bort varandra
                    new List<Entity>()
                    {
                        new Shooter(new Sprite(textureHullRotating), new Vector2(123,325), 0, 1, 1f, 1000f, 50f, 10,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 1, 0, 100f, 3, 1),
                        true, true, 0.1f, 0.05f, 30f),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(325,325), 0, 1, 1f, 1000f, 50f, 10,
                            new Projectile(new Sprite(textureProjectile), new Vector2(50,50), 0, 1, 0, 100f, 3, 1),
                        true, true, 0.1f, 0.05f, 30f)
                    }/*, controllers[0]*/)
            );
            backgrounds = new List<Background>(){
                new Background(
                    new List<Entity>()
                    {
                        new Entity(new Sprite(textureSolar), new Vector2(0, 0), 0, 1, 1f, 1000f, true, false, 0.1f, 0.05f, 30f)
                    },
                    0.3f, Camera)
                
            };
            Random r = new Random();
            controllers.Add(
                new WrappingBackground(
                    new List<Entity>()
                    {
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f),
                        new Entity(new Sprite(textureCloudCreepyBlurry), new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), 0, 2f, 1f, 1000f, true, false, 0.05f, 1, 0, elasticity: 0.0f)
                    }, 0.7f, Camera)
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
            foreach(Background b in backgrounds)
                b.Update(gameTime);
            Camera.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);
            _spriteBatch.Begin();
            //background.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: Camera.Transform);
            //Vector2 CameraPosition = controllers[0].Position - new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.5f);
            // Matrix m = Matrix.CreateTranslation(new Vector3(-CameraPosition, 0));
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, m);
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, Matrix.CreateScale(0.5f));
            foreach(Background b in backgrounds)
                b.Draw(_spriteBatch, Matrix.Identity);
            foreach (Controller c in controllers)
                c.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
