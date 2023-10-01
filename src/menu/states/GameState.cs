using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.controllers;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.menu.states.menu_states;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkIO.src.menu.states
{
    public class GameState : State
    {
        public Player Player { get; protected set; }
        public Camera Camera { get; protected set; }
        protected List<IControllable> controllers;
        protected List<Background> backgrounds;
        protected State previousState;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input, [OptionalAttribute]State previousState, [OptionalAttribute] List<IControllable> controllers) : base(game, graphicsDevice, content, input)
        {
            this.previousState = previousState;
            Player = new Player(new List<IControllable>(), input);
            Camera = Player.Camera;
            this.controllers = new List<IControllable>();
            this.backgrounds = new List<Background>();
            if(controllers!= null)
                foreach (IControllable c in controllers)
                    this.controllers.Add((IControllable)c.Clone());


        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: Player.Camera.Transform);
            game.GraphicsDevice.Clear(Color.DarkGray);
            //Vector2 CameraPosition = p.Position - new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.5f);
            // Matrix m = Matrix.CreateTranslation(new Vector3(-CameraPosition, 0));
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, m);
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, Matrix.CreateScale(0.5f));
            foreach (Background b in backgrounds)
            {
                b.Draw(spriteBatch);
            }

            foreach (IControllable c in controllers)
            {
                c.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public override void PostUpdate()
        {
            //throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (Player.Input.PauseClicked)
                game.ChangeState(new PauseState(game, graphicsDevice, content, this, input));
            else if (Player.Input.BuildClicked)
                if (Player.controllables != null && Player.controllables.Count>0)
                    game.ChangeState(new BuildOverviewState(game, graphicsDevice, content, this, input, Player));
            if (Keyboard.GetState().IsKeyDown(Keys.Back) && previousState != null)
                game.ChangeState(previousState);
            RunGame(gameTime);
        }

        public void RunGame(GameTime gameTime)
        {
            foreach (IControllable c in controllers)
                c.Update(gameTime);
            foreach (IControllable c1 in controllers)
                foreach (IControllable c2 in controllers)
                    if (c1 != c2)
                        c1.Collide(c2);
            foreach (Background b in backgrounds)
                b.Update(gameTime);
        }
    }
}
