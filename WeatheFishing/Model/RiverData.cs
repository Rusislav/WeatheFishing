using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatheFishing.Model;

namespace WeatheFishing.Model
{
    public class RiverData
    {
        public string River { get; set; }
        public string HydrometricStation { get; set; }
        public double MinimumFloatRateOfTheRiver { get; set; }
        public double NominalFloatRateOfTheRiver { get; set; }
        public double MaximumFloatRateOfTheRiver { get; set; }
        public int DepthOfTheWater { get; set; }
        public double FlowRate { get; set; }
        public int ChangeInHeightOfTheRiver { get; set; }
    }
}

