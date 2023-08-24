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
using System.Text;

namespace NetworkIO.src.menu.states
{
    public abstract class GameState : State
    {
        public Player Player { get; protected set; }
        protected List<Controller> controllers;
        protected List<Background> backgrounds;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            Player = new Player(new List<Entity>());
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

            foreach (Controller c in controllers)
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
            if (Player.PauseClicked)
                game.ChangeState(new PauseState(game, graphicsDevice, content, this));
            else if (Player.BuildClicked)
                game.ChangeState(new BuildState(game, graphicsDevice, content, this));
            else
            {
                RunGame(gameTime);
            }
        }

        public void RunGame(GameTime gameTime)
        {
            foreach (Controller c in controllers)
                c.Update(gameTime);
            foreach (Controller c1 in controllers)
                foreach (Controller c2 in controllers)
                    if (c1 != c2)
                        c1.Collide(c2);
            foreach (Background b in backgrounds)
                b.Update(gameTime);
        }
    }
}
