using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.controllers
{
    public class CompositeController : Controller
    {
        public CompositeController(List<Entity> entities) : base(entities)
        {
        }

        public override void SetEntities(List<Entity> newEntities) //TODO
        {
            if (newEntities != null)
            {
                List<Entity> oldEntities = entities;
                entities = new List<Entity>();
                foreach (Entity e in newEntities)
                    AddEntity(e);
                if (entities.Count == 0)
                {
                    entities = oldEntities;
                }
            }
        }

        public void AddEntity(Entity e)
        {
            if (e != null)
            {
                entities.Add(e);
                UpdatePosition();
                UpdateRadius();
            }

        }
    }
}
