using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Charts.Interfaces
{
    public interface IChartDataViewModel
    {
        public string Category { get; }
        public double Value { get; }
        public string FormattedValue { get; }
    }
}
