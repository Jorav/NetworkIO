﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NetworkIO.src.factories;
using NetworkIO.src.movable;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class MenuController : Controller
    {
        public Camera Camera { get; private set; }
        public List<IControllable> oldControllables;
        public MenuController(List<IControllable> collidables) : base(collidables)
        {
            oldControllables = controllables;
            Camera = new Camera(this, true);
        }

        public void AddOpenLinks()
        {
            if (controllables.Count == 1 && controllables[0] is EntityController ec)
            {
                ec.AddAvailableLinkDisplays();
            }
        }

        public void ClearOpenLinks()
        {
            if (controllables.Count == 1 && controllables[0] is EntityController ec)
            {
                ec.ClearAvailableLinks();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Camera.Update();
        }

        public void FocusOn(IControllable c)
        {
            if (c is Controller cc)
                controllables = cc.controllables;
            else if (c is EntityController ec)
                SetControllables(new List<IControllable>(ec.entities));
            else if (c is WorldEntity we)
                SetControllables(new List<IControllable>{ we });
        }

        public void Reset()
        {
            ClearOpenLinks();
            controllables = oldControllables;
        }

        /**
         * returns the entity clicked by player, null if no entity was clicked
         */
        public IControllable ControllableClicked()
        {
            foreach (IControllable c in controllables)
            {
                IControllable clicked = c.ControllableContainingInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform); //if (c is EntityController ec && ec.ContainsInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform))
                if (clicked != null)
                    return c;
            }
            return null;
        }
        public WorldEntity EntityClicked()
        {
            foreach (IControllable c in controllables)
            {
                IControllable clicked = c.ControllableContainingInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform); //if (c is EntityController ec && ec.ContainsInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform))
                if (clicked != null && clicked is WorldEntity clickedE)
                    return clickedE;
            }
            return null;
        }

        public void ReplaceEntity(WorldEntity oldEntity, WorldEntity newEntity)
        {
            foreach (IControllable c in controllables)
            {
                if(c is EntityController ec)
                ec.ReplaceEntity(oldEntity, newEntity);
            }
        }
    }
}
