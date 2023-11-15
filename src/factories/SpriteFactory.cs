using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.factories
{
    public class SpriteFactory
    {
        public static Texture2D rectangularHull;
        public static Texture2D circularHull;
        public static Texture2D gun;
        public static Texture2D projectile;
        public static Texture2D cloud;
        public static Texture2D sun;
        public static Texture2D emptyLink;
        public static Texture2D spike;
        public static Texture2D entityButton;
        public static Texture2D linkHull;
        public static Texture2D triangularEqualLeggedHull;
        public static Texture2D triangular90AngleHull;
        public static Texture2D engine;

        public static Sprite Create(Vector2 position, IDs id)
        {
            Vector2 defaultPosition = Vector2.Zero;
            switch (id)
            {
                case IDs.ENTITY_DEFAULT: return new Sprite(rectangularHull);
                case IDs.EMPTY_LINK: return new Sprite(emptyLink);

                case IDs.COMPOSITE: return new Sprite(rectangularHull);
                case IDs.CIRCULAR_COMPOSITE: return new Sprite(circularHull);
                case IDs.LINK_COMPOSITE: return new Sprite(linkHull);
                case IDs.TRIANGULAR_EQUAL_COMPOSITE: return new Sprite(triangularEqualLeggedHull);
                case IDs.TRIANGULAR_90ANGLE_COMPOSITE: return new Sprite(triangular90AngleHull);

                case IDs.SHOOTER: return new Sprite(gun);
                case IDs.PROJECTILE: return new Sprite(projectile);
                case IDs.SPIKE: return new Sprite(spike);
                case IDs.ENGINE: return new Sprite(engine);

                #region background
                case IDs.CLOUD: return new Sprite(cloud);
                case IDs.SUN: return new Sprite(sun);
                #endregion

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
