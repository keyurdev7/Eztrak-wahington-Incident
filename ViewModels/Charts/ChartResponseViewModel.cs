using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Charts.Interfaces;

namespace ViewModels.Charts
{
    public class ChartResponseViewModel
    {
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = "Some error occured, please try again later.";
        public List<IChartDataViewModel> Data { get; set; } = new();
    }
}
