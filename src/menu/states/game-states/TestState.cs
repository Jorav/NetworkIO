using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.controllers;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.factories;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.menu.states.game_states
{
    class TestState : GameState
    {
        public TestState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, Input input) : base(game, graphicsDevice, content, input)
        {
            //Game1.Player = new Player(input);
            /**Player = new Player(
            new List<ICollidable>()
            {
                        
                        new Shooter(new Sprite(textureSprayGun), new Vector2(0,0),
                            new Projectile(new Sprite(textureProjectile), new Vector2(0,0))),
                        new Shooter(new Sprite(textureHullRotating), new Vector2(0,100),
                            new Projectile(new Sprite(textureProjectile), new Vector2(0,100)))
                        EntityFactory.Create(new Vector2(0,0), IDs.COMPOSITE)

                //EntityFactory.Create(new Vector2(0,0), IDs.SHOOTER),
                //EntityFactory.Create(new Vector2(10,10), IDs.SHOOTER),
                //EntityFactory.Create(new Vector2(20,20), IDs.SHOOTER),

            }, input);
            ((RectangularComposite)Player.collidables[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 0);
            ((RectangularComposite)Player.collidables[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 1);
            ((RectangularComposite)Player.collidables[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 2);
            ((RectangularComposite)Player.collidables[0]).AddEntity(EntityFactory.Create(new Vector2(0, 0), IDs.SHOOTER), 3);*/
            
            /*Player = new Player(
            new List<Entity>()
            {
                EntityFactory.Create(new Vector2(0,0), IDs.SHOOTER),
                EntityFactory.Create(new Vector2(10,10), IDs.SHOOTER),
                EntityFactory.Create(new Vector2(20,20), IDs.SHOOTER),

            }, input);*/
            Random r = new Random();
            ChaserAI chaser = new ChaserAI(new Vector2(500,500), Player);
            controllers.Add(chaser);
            /*
            controllers.Add(new ChaserAI(
                     new List<IController>()
                     {
                        EntityFactory.Create(new Vector2(-100,-100),IDs.SHOOTER),
                        EntityFactory.Create(new Vector2(-200,-200),IDs.SHOOTER),
                        EntityFactory.Create(new Vector2(-300,-300),IDs.SHOOTER)
                     }, Player)
             );
            controllers.Add(new IndecisiveAI(
                    new List<IController>()
                    {
                        EntityFactory.Create( new Vector2(467,213), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(212,512), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(512,413), IDs.SHOOTER)
                    })
            );
            controllers.Add(new RandomAI(
                    new List<IController>()
                    {
                        EntityFactory.Create( new Vector2(886,1243), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(241,253), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(354,-3), IDs.SHOOTER)
                    })
            );
            controllers.Add(new CircularAI(
                    new List<IController>()
                    {
                        EntityFactory.Create( new Vector2(200,500), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(600,600), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(300,300), IDs.SHOOTER),
                    })
            );
            controllers.Add(new CollidablesController( //att lägga till en till fick det att explodera typ... weird. och de trycker inte bort varandra
                    new List<IController>()
                    {
                        EntityFactory.Create( new Vector2(123,325), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(325,325), IDs.SHOOTER),
                        EntityFactory.Create( new Vector2(325,125), IDs.SHOOTER),
                    }/*, p)
            );*/
            backgrounds = new List<Background>(){
                new WrappingBackground(
                    new List<IControllable>()
                    {
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.SUN),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.SUN),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.SUN),
                    },
                    0.3f, Player.Camera)
            };
            backgrounds.Add(new WrappingBackground(
                    new List<IControllable>()
                    {
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD),
                        EntityFactory.Create(new Vector2((float)(r.NextDouble()-0.5)*Game1.ScreenWidth, (float)(r.NextDouble()-0.5)*Game1.ScreenHeight), IDs.CLOUD)
                    }, 0.6f, Player.Camera, new Vector2(0.1f,0.1f))
            );
        }
    }
}
