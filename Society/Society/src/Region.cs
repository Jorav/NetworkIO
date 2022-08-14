using System;
using System.Collections.Generic;
using System.Text;

namespace Society.src
{
    public class Region
    {
        private TileImprovement tileImprovement;
        private List<Civilian> civilians;
        private List<Troop> troops;
        private Resource[] resources;

        public Region()
        {
            tileImprovement = null;
            civilians = new List<Unit>();
        }

        public Region(TileImprovement tileImprovement, Unit units)
        {
            this.tileImprovement = tileImprovement;
            this.civilians = units;
        }

        public void GenerateResources()
        {
            resources = tileImprovement.GenerateResource(civilians.Count);
        }
    }
}
