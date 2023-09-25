using Microsoft.Xna.Framework;
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
        Input input;
        public bool clickedOutside;
        private bool previouslyMBDown;
        public IControllable controllableClicked;
        public bool requireNewClick;
        public bool addEntity;
        public bool removeEntity;
        public MenuController(List<IControllable> collidables, Input input) : base(collidables)
        {
            oldControllables = controllables;
            Camera = new Camera(this, true);
            this.input = input;
            requireNewClick = true;
        }

        public void AddOpenLinks()
        {
            foreach (IControllable c in controllables)
                if (c is EntityController ec)
                    ec.AddAvailableLinkDisplays();
        }

        public void ClearOpenLinks()
        {
            foreach (IControllable c in controllables)
                if (c is EntityController ec)
                    ec.ClearAvailableLinks();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Camera.Update();
            if (!requireNewClick)
            {
                if (input.LeftMBDown || input.RightMBDown)
                {
                    if (controllables[0] is Controller)
                        controllableClicked = ControllableClicked();
                    else if (controllables[0] is EntityController)
                        controllableClicked = EntityClicked();
                    if (!previouslyMBDown && controllableClicked == null)
                        clickedOutside = true;
                    else if (previouslyMBDown && controllableClicked != null)
                    {
                        if (input.LeftMBDown)
                            addEntity = true;
                        else if (input.RightMBDown)
                            removeEntity = true;
                    }
                }
                previouslyMBDown = input.LeftMBDown || input.RightMBDown;
            }
            else
                requireNewClick = input.LeftMBDown || input.RightMBDown;
        }

        public void FocusOn(IControllable c)
        {
            if (c is Controller cc)
                controllables = cc.controllables;
            else if (c is EntityController ec)
                SetControllables(new List<IControllable>(ec.Entities));
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

        public bool ReplaceEntity(WorldEntity oldEntity, WorldEntity newEntity)
        {
            foreach (IControllable c in controllables)
            {
                if(c is EntityController ec)
                    return ec.ReplaceEntity(oldEntity, newEntity);
            }
            return false;
        }

        public void RemoveEntity(WorldEntity clickedE)
        {
            foreach (IControllable c in controllables)
            {
                if (c is EntityController ec)
                    ec.RemoveEntity(clickedE);
            }
        }
    }
}
