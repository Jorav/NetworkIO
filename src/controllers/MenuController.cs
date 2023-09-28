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
        public MenuController(List<IControllable> collidables, Input input) : base(collidables)
        {
            oldControllables = controllables;
            Camera = new Camera(this, true);
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
            Camera.Position = Position;
            Camera.Update();

            bool newClick = ((input.LeftMBDown && !previouslyLeftMBDown) || (input.RightMBDown && !previouslyRightMBDown));

            //get entity affected
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

            //left click
            if (input.LeftMBDown)
            {
                if (controllableClicked == null && newClick)
                {
                    clickedOutside = true;
                    newClickRequired = true;
                }
                    
                else if (controllableClicked != null && !newClickRequired)
                {
                    addControllable = true; 
                }
            }


            //right click
            else
            {
                if (input.RightMBDown && controllableClicked != null)
                {
                    removeEntity = true;
                    
                }
                if (!input.RightMBDown && previouslyRightMBDown && controllables.Count != 1)
                {
                    /*AddSeperatedEntities();
                    RemoveEmptyControllers();
                    UpdatePosition();
                    UpdateRadius();*/
                    clickedOutside = true;
                    newClickRequired = true;

                }
                //else if (!input.RightMBDown && previouslyRightMBDown && controllables.Count == 1)
                    //AddOpenLinks();
            }

            //new click required
            if (newClickRequired && newClick)
                newClickRequired = false;
            
            previouslyLeftMBDown = input.LeftMBDown;
            previouslyRightMBDown = input.RightMBDown;
        }

        public void FocusOn(IControllable c)
        {
            if (c is Controller cc)
                controllables = cc.controllables;
            else if (c is EntityController ec)
                SetControllables(new List<IControllable>(ec.Entities));
            else if (c is WorldEntity we)
                SetControllables(new List<IControllable> { we });
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
                    ec.RemoveEntity(clickedE);
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
