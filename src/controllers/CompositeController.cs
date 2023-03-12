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
    }
}
