using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ReStageViewModel
    {
        public List<ReStageListViewModel> Transactions { get; set; } = new();

    }
    public class ReStageListViewModel : TransactionIssueViewModel
    {
        public LocationBriefViewModel NewLocation { get; set; } = new();
        public double NewQuantity { get; set; }
    }
}
