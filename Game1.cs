using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src;
using NetworkIO.src.controllers;
using NetworkIO.src.entities;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.menu.states;
using NetworkIO.src.menu.states.menu_states;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;

namespace NetworkIO
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int ScreenWidth;
        public static int ScreenHeight;

        private State currentState;
        private State nextState;
        public Input Input { get; set; }

        public void ChangeState(State state)
        {
            nextState = state;
        }

        public State GetNextState()
        {
            if (nextState == null)
                return currentState;
            return nextState;
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Input = new Input()
            {
                Up = Keys.W,
                Down = Keys.S,
                Left = Keys.A,
                Right = Keys.D,
                Pause = Keys.Escape,
                Build = Keys.Enter,
            };
            currentState = new MainMenu(this, GraphicsDevice, Content, Input);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _graphics.ApplyChanges();

            Texture2D textureHullRotating = Content.Load<Texture2D>("parts/RotatingHull");
            //Texture2D textureSprayGun = Content.Load<Texture2D>("parts/SprayGun");
            Texture2D textureGun = Content.Load<Texture2D>("parts/Gun");
            Texture2D textureProjectile = Content.Load<Texture2D>("projectiles/BulletWhite");
            Texture2D textureSun = Content.Load<Texture2D>("background/solar");
            Texture2D textureCloudCreepy = Content.Load<Texture2D>("background/cloud_creepy");
            Texture2D textureCloudCreepyBlurry = Content.Load<Texture2D>("background/cloud_creepy_blurry");
            Texture2D textureEmptyLink = Content.Load<Texture2D>("parts/empty_link_directed");
            Texture2D textureSpike = Content.Load<Texture2D>("parts/spike");
            Texture2D textureEntityButton = Content.Load<Texture2D>("controls/entityButton");
            Texture2D textureCircularHull = Content.Load<Texture2D>("parts/CircularHull");
            Texture2D textureLinkHull = Content.Load<Texture2D>("parts/LinkHull");
            EntityFactory.rectangularHull = textureHullRotating;
            EntityFactory.circularHull = textureCircularHull;
            EntityFactory.gun = textureGun;
            EntityFactory.projectile = textureProjectile;
            EntityFactory.cloud = textureCloudCreepy;
            EntityFactory.sun = textureSun;
            EntityFactory.emptyLink = textureEmptyLink;
            EntityFactory.spike = textureSpike;
            EntityFactory.entityButton = textureEntityButton;
            EntityFactory.linkHull = textureLinkHull;

        }


        protected override void Update(GameTime gameTime)
        {
            if (nextState != null)
            {
                currentState = nextState;
                nextState = null;
            }

            currentState.Update(gameTime);
            currentState.PostUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            currentState.Draw(gameTime, _spriteBatch);
            base.Draw(gameTime);
        }
    }
}
