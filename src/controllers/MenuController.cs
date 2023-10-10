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
        private bool previouslyLeftMBDown;
        private bool previouslyRightMBDown;
        public IControllable controllableClicked;
        public bool newClickRequired;
        public bool addControllable;
        public bool removeEntity;
        //public override Vector2 Position { get { return base.Position; } set { base.Position = value; Camera.Position = value; } }
        public MenuController(List<IControllable> collidables, Input input) : base(collidables)
        {
            oldControllables = controllables;
            Camera = new Camera(this, true);
            Camera.AutoAdjustZoom = true;
            Camera.Position = Position;
            this.input = input;
            newClickRequired = true;
            previouslyLeftMBDown = input.LeftMBDown;
            previouslyRightMBDown = input.RightMBDown;
    }

        public void AddOpenLinks()
        {
            foreach (IControllable c in controllables)
                if (c is EntityController ec)
                    ec.AddAvailableLinkDisplays();
            UpdatePosition();
            UpdateRadius();
        }

        public void ClearOpenLinks()
        {
            foreach (IControllable c in controllables)
                if (c is EntityController ec)
                    ec.ClearAvailableLinks();
            UpdatePosition();
            UpdateRadius();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Camera.Update();

            //get entity affected
            UpdateControllableClicked();
            HandleLeftClick();
            HandleRightClick();

            //new click required
            if (newClickRequired && ((input.LeftMBDown && !previouslyLeftMBDown) || (input.RightMBDown && !previouslyRightMBDown)))
                newClickRequired = false;

            previouslyLeftMBDown = input.LeftMBDown;
            previouslyRightMBDown = input.RightMBDown;
        }

        private void HandleRightClick()
        {
            if (input.RightMBDown && controllableClicked != null)
            {
                removeEntity = true;

            }
            if (!input.RightMBDown && previouslyRightMBDown)
            {
                if (controllables.Count != 1)
                {
                    clickedOutside = true;
                    newClickRequired = true;
                }
                else
                {
                    AddOpenLinks();
                }
                /*AddSeperatedEntities();
                RemoveEmptyControllers();
                UpdatePosition();
                UpdateRadius();*/

            }
        }

        private void HandleLeftClick()
        {
            if (input.LeftMBDown)
            {
                if (controllableClicked == null && input.LeftMBDown && !previouslyLeftMBDown)
                {
                    clickedOutside = true;
                    newClickRequired = true;
                }

                else if (controllableClicked != null && !newClickRequired)
                {
                    addControllable = true;
                }
            }
        }

        private void UpdateControllableClicked()
        {
            if (input.LeftMBDown || input.RightMBDown)
            {
                foreach (IControllable c in controllables)
                {
                    if (c is Controller)
                        controllableClicked = ControllableClicked();
                    else if (c is EntityController)
                        controllableClicked = EntityClicked();
                }
            }
            else
            {
                controllableClicked = null;
            }
        }

        public void FocusOn(IControllable c)
        {/*
            ClearOpenLinks();
            if (c is Controller cc)
            {
                oldControllables.Push(controllables);
                SetControllables(cc.controllables);
                controllerEdited = cc;
            }
            else if (c is EntityController ec)
            {
                oldControllables.Push(controllables);
                //oldControllables.Push(controllables);
                SetControllables(new List<IControllable>() { ec });
                //SetControllables((List<IControllable>)(ec.Entities));
                AddOpenLinks();
                maxZoom = true;
            }

            else if (c is WorldEntity we)
            {
                oldControllables.Push(controllables);
                if (we.EntityController != null)
                    SetControllables(new List<IControllable> { we.EntityController });
                else
                    SetControllables(new List<IControllable> { we });
                AddOpenLinks();
                maxZoom = true;
            }
            Camera.InBuildScreen = true;
            newClickRequired = true;
        }
        public void DeFocus()
        {
            ClearOpenLinks();
            if (oldControllables.Count != 0)
            {
                List<IControllable> controllables = this.controllables;
                SetControllables(oldControllables.Pop());
                if (controllerEdited != null)
                    controllerEdited.SetControllables(controllables);
                else
                    //AddControllable(c);
                    foreach (IControllable c in controllables)
                        if (!this.controllables.Contains(c))
                            AddControllable(c);
                maxZoom = false;
            }
            Camera.InBuildScreen = true;
        }*/
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
                if (c is EntityController ec)
                {
                    bool replaced = ec.ReplaceEntity(oldEntity, newEntity);
                    if (replaced)
                    {
                        ClearOpenLinks();
                        AddOpenLinks();
                        return replaced;
                    }
                }

            }
            return false;
        }

        public void RemoveEntity(WorldEntity clickedE)
        {
            ClearOpenLinks();
            foreach (IControllable c in controllables)
            {
                if (c is EntityController ec)
                    ec.Remove(clickedE);
            }
            ClearOpenLinks();
            AddSeperatedEntities();
            RemoveEmptyControllers();
            //AddOpenLinks();
            UpdatePosition();
            UpdateRadius();
        }
    }
}
