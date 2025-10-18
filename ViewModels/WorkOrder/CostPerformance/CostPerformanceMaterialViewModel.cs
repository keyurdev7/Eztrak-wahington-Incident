using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.WorkOrder.CostPerformance
{
    public class CostPerformanceMaterialViewModel
    {
        public InventoryBriefViewModel Material { get; set; } = new();
        public double Cost { get; set; }
        public double ActualCost { get; set; }
        public double Quantity { get; set; }
        public string UOM { get; set; }
    }
}
