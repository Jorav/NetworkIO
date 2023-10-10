using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src
{
    public interface IController : IControllable
    {
        public bool Remove(IControllable c);
        public List<IControllable> Controllables { get; set; }
    }
}
