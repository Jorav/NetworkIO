using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src;
using NetworkIO.src.controllers;
using NetworkIO.src.entities;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.utility;
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

        public static int ScreenWidth;
        public static int ScreenHeight;
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

            Texture2D textureHullRotating = Content.Load<Texture2D>("parts/Hull_rotating");
            Texture2D textureSprayGun = Content.Load<Texture2D>("parts/SprayGun");
            Texture2D textureProjectile = Content.Load<Texture2D>("projectiles/Bullet");
            Texture2D textureSun = Content.Load<Texture2D>("background/solar");
            Texture2D textureCloudCreepy = Content.Load<Texture2D>("background/cloud_creepy");
            Texture2D textureCloudCreepyBlurry = Content.Load<Texture2D>("background/cloud_creepy_blurry");
            EntityFactory.hull = textureHullRotating;
            EntityFactory.gun = textureHullRotating;
            EntityFactory.projectile = textureProjectile;
            EntityFactory.cloud = textureCloudCreepy;
            EntityFactory.sun = textureSun;

            //TODO: add factories to replace this bloated code
            Player p = new Player(
                    new List<Entity>()
                    {
                        /**
                        new Shooter(new Sprite(textureSprayGun), new Vector2(0,0),
                            new Projectile(new Sprite(textureProjectile), new Vector2(0,0))),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(0,100),
                            new Projectile(new Sprite(textureProjectile), new Vector2(0,100)))*/
                        EntityFactory.Create(new Vector2(0,0), IDs.COMPOSITE)

                        //EntityFactory.Create(new Vector2(0,0), IDs.SHOOTER),
                        //EntityFactory.Create(new Vector2(10,10), IDs.SHOOTER),
                        //EntityFactory.Create(new Vector2(20,20), IDs.SHOOTER),

                    });
            ((RectangularComposite)p.entities[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 0);
            ((RectangularComposite)p.entities[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 1);
            ((RectangularComposite)p.entities[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 2);
            ((RectangularComposite)p.entities[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 3);
            Camera = p.Camera;
            controllers = new List<Controller>();
            Random r = new Random();

            controllers.Add(
                new WrappingBackground(
                    new List<Entity>()
                    {
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.CLOUD)
                    }, 0.6f, Camera)
            );
            
            controllers.Add(p);

            controllers.Add(new ChaserAI(
                     new List<Entity>()
                     {
                        EntityFactory.Create(new Vector2(-100,-100),IDs.SHOOTER),
                        EntityFactory.Create(new Vector2(-200,-200),IDs.SHOOTER),
                        EntityFactory.Create(new Vector2(-300,-300),IDs.SHOOTER)
                     }, p)
             );
            controllers.Add(new IndecisiveAI(
                    new List<Entity>()
                    {
                        EntityFactory.Create( new Vector2(467,213), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(212,512), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(512,413), IDs.SHOOTER)
                    })
            );
            controllers.Add(new RandomAI(
                    new List<Entity>()
                    {
                        EntityFactory.Create( new Vector2(886,1243), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(241,253), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(354,-3), IDs.SHOOTER)
                    })
            );
            controllers.Add(new CircularAI(
                    new List<Entity>()
                    {
                        EntityFactory.Create( new Vector2(200,500), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(600,600), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(300,300), IDs.SHOOTER),
                    })
            );
            controllers.Add(new Controller( //att lägga till en till fick det att explodera typ... weird. och de trycker inte bort varandra
                    new List<Entity>()
                    {
                        EntityFactory.Create( new Vector2(123,325), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(325,325), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(325,125), IDs.SHOOTER),
                    }/*, p*/)
            );
            backgrounds = new List<Background>(){
                new WrappingBackground(
                    new List<Entity>()
                    {
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.SUN),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.SUN),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*ScreenWidth, (float)(r.NextDouble()-0.5)*ScreenHeight), IDs.SUN),
                    },
                    0.3f, Camera)
            };

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
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(transformMatrix: Camera.Transform);
            GraphicsDevice.Clear(Color.DarkGray);
            //Vector2 CameraPosition = p.Position - new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.5f);
            // Matrix m = Matrix.CreateTranslation(new Vector3(-CameraPosition, 0));
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, m);
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, Matrix.CreateScale(0.5f));
            foreach(Background b in backgrounds)
            {
                b.Draw(_spriteBatch, Matrix.Identity);
            }

            foreach (Controller c in controllers) {
                c.Draw(_spriteBatch, Matrix.Identity);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
