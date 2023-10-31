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
    public class MenuController : CohesiveController
    {
        public Camera Camera { get; private set; }
        Input input;
        public bool clickedOutside;
        private bool previouslyLeftMBDown;
        private bool previouslyRightMBDown;
        public IControllable controllableClicked;
        public bool newClickRequired;
        public bool clickedOnControllable;
        public bool removeEntity;
        public bool focusOn;
        public bool maxZoom;
        private Stack<(IController, List<IControllable>)> previousControllables;
        //public override Vector2 Position { get { return base.Position; } set { base.Position = value; Camera.Position = value; } }
        public MenuController(List<IControllable> collidables, Input input) : base(collidables)
        {
            Camera = new Camera(this, true);
            Camera.AutoAdjustZoom = true;
            Camera.Position = Position;
            this.input = input;
            newClickRequired = true;
            previouslyLeftMBDown = input.LeftMBDown;
            previouslyRightMBDown = input.RightMBDown;
            previousControllables = new Stack<(IController, List<IControllable>)>();
            integrateSeperatedEntities = true;
        }

        public void AddOpenLinks()
        {
            foreach (IControllable c in Controllables)
                if (c is EntityController ec)
                    ec.AddAvailableLinkDisplays();
            UpdatePosition();
            UpdateRadius();
        }

        public void ClearOpenLinks()
        {
            foreach (IControllable c in Controllables)
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
                if (Controllables.Count != 1)
                {
                    clickedOutside = true;
                    newClickRequired = true;
                }
                else if (maxZoom)
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
                if (controllableClicked != null && !newClickRequired)
                {
                    clickedOnControllable = true;
                }
                else if (controllableClicked == null && input.LeftMBDown && !previouslyLeftMBDown && previousControllables.Count != 0)
                {
                    clickedOutside = true;
                    newClickRequired = true;
                } 
            }
        }

        private void UpdateControllableClicked()
        {
            if (input.LeftMBDown || input.RightMBDown)
            {
                foreach (IControllable c in Controllables)
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
        {
            if (!maxZoom)
            {
                ClearOpenLinks();
                if (c is Controller cc)
                {//YOU HAVE TO ADD THE SPECIFIC CONTROLLER TO PREVIOUSCONTROLLABLES INSTEAD OF THE LIST SO THAT YOU CAN ADD ENTITYCONTROLLER DIRECTLY TO IT
                    previousControllables.Push((cc,Controllables));
                    Controllables = cc.Controllables;
                    //Controllables = new List<IControllable>(cc.Controllables.ToArray());
                    
                }
                else if (c is EntityController ec)
                {
                    previousControllables.Push((ec,Controllables));
                    Controllables = new List<IControllable> { ec };
                    AddOpenLinks();
                }

                else if (c is WorldEntity we)
                {
                    //if (we.Manager != null)
                    //Controllables = new List<IControllable> { we.Manager };
                    if (we.Manager != null && we.Manager is EntityController e)
                    {
                        previousControllables.Push((e,Controllables));
                        Controllables = new List<IControllable> { e };
                    }

                    else
                    {
                        previousControllables.Push((this,Controllables));
                        Controllables = new List<IControllable> { we };
                    }
                        
                    
                    AddOpenLinks();
                    maxZoom = true;
                }
                Camera.InBuildScreen = true;
                newClickRequired = true;
                Camera.Update();
            }
        }

        public void Reset()
        {
            while (previousControllables.Count < 0)
                DeFocus();
        }

        public void DeFocus()
        {
            ClearOpenLinks();
            maxZoom = false;
            if (previousControllables.Count != 0)
            {
                (IController, List<IControllable>) oldList = previousControllables.Pop();
                if (oldList.Item1 is EntityController ec) {
                    foreach (IControllable c in Controllables)
                    {
                        if (!oldList.Item2.Contains(c))
                            oldList.Item2.Add(c);
                    }
                    Controllables = oldList.Item2;
                }
                else if (oldList.Item1 is Controller cc)
                {
                    /*foreach (IControllable c in Controllables)
                    {
                        if (!cc.Controllables.Contains(c))
                            cc.Controllables.Add(c);
                    }*/
                    foreach (IControllable c in Controllables)
                    {
                        if (!cc.Controllables.Contains(c))
                            cc.Controllables.Add(c);
                    }
                    Controllables = oldList.Item2;
                }
                //if (c is Entity e)
                //e.Manager.Controllables.Add(e);
                /*if (c.Manager is Controller && !oldList.Contains(c))
                    if (oldList.Contains(c.Manager))
                        c.Manager.Controllables.Add(c);
                    else
                        oldList.Add(c);*/

            }
            Camera.InBuildScreen = true;
            newClickRequired = true;
            Camera.Update();
        }

        /**
         * returns the entity clicked by player, null if no entity was clicked
         */
        public IControllable ControllableClicked()
        {
            foreach (IControllable c in Controllables)
            {
                IControllable clicked = c.ControllableContainingInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform); //if (c is EntityController ec && ec.ContainsInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform))
                if (clicked != null)
                    return c;
            }
            return null;
        }
        public WorldEntity EntityClicked()
        {
            foreach (IControllable c in Controllables)
            {
                IControllable clicked = c.ControllableContainingInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform); //if (c is EntityController ec && ec.ContainsInSpace(Mouse.GetState().Position.ToVector2(), Camera.Transform))
                if (clicked != null && clicked is WorldEntity clickedE)
                    return clickedE;
            }
            return null;
        }

        public bool ReplaceEntity(WorldEntity oldEntity, WorldEntity newEntity)
        {
            foreach (IControllable c in Controllables)
            {
                if (c is EntityController ec)
                {
                    bool replaced = ec.ReplaceEntity(oldEntity, newEntity);
                    if (replaced)
                    {
                        ClearOpenLinks();
                        AddOpenLinks();
                        ec.Manager = this;
                        return replaced;
                    }
                }

            }
            return false;
        }

        public void RemoveEntity(WorldEntity clickedE)
        {
            ClearOpenLinks();
            foreach (IControllable c in Controllables)
            {
                if (c is EntityController ec)
                    ec.Remove(clickedE);
            }
            ClearOpenLinks();
            //AddSeperatedEntities();
            RemoveEmptyControllers();
            //AddOpenLinks();
            UpdatePosition();
            UpdateRadius();
        }
    }
}
