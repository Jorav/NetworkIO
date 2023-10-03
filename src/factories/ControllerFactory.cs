using Microsoft.Xna.Framework;
using NetworkIO.src.controllers;
using NetworkIO.src.utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.factories
{
    public class ControllerFactory
    {

        public static Controller Create(Vector2 position, IDs id)
        {
            switch (id)
            {
                case IDs.CONTROLLER_DEFAULT: return new Controller(position);
                case IDs.CHASER_AI: return new ChaserAI(position);
                case IDs.Player: Controller c = new Controller(position); return c;
                default:
                    throw new NotImplementedException();
            }
        }

        public static String GetName(IDs id)
        {
            switch (id)
            {
                case IDs.CONTROLLER_DEFAULT: return Controller.GetName();
                case IDs.CHASER_AI: return ChaserAI.GetName();
                case IDs.Player: return Player.GetName();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
