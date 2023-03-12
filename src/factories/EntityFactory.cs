using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.entities;
using NetworkIO.src.entities.hulls;
using NetworkIO.src.utility;
using System;

namespace NetworkIO.src.factories
{
    public class EntityFactory
    {
        public static Texture2D hull;
        public static Texture2D gun;
        public static Texture2D projectile;
        public static Texture2D cloud;
        public static Texture2D sun;

        public static Entity Create(Vector2 position, IDs id)
        {
            Vector2 defaultPosition = Vector2.Zero;
            switch (id)
            {
                case IDs.ENTITY_DEFAULT: return new Entity(new Sprite(hull), position);
                case IDs.COMPOSITE: return new RectangularComposite(new Sprite(hull), position);
                case IDs.SHOOTER: return new Shooter(new Sprite(gun), position, (Projectile)Create(position, IDs.PROJECTILE));
                case IDs.PROJECTILE: return new Projectile(new Sprite(projectile), position);
                //case (int)IDs.COMPOSITE: return new Composite(new Sprite(hull), position);

                #region background
                case IDs.CLOUD: return new Entity(new Sprite(cloud), position);
                case IDs.SUN: return new Entity(new Sprite(sun), position, isCollidable:false);
                #endregion

                default:
                    throw new NotImplementedException();
            }
        }
        /**
        public static void LoadTextures(Texture2D hull, Texture2D gun, Texture2D projectile, Texture2D cloud) //TODO - add support for multiple skins
        {
            this.hull = hull;
            this.gun = gun;
            this.projectile = projectile;
            this.cloud = cloud;
        }*/
    }
}
