using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class WorkOrderBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public override string? Select2Text { get; set; }
    }
    public class WorkOrderForIssueBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public WorkOrderForIssueBriefViewModel() : base(true, "Work order is required")
        {

        }
        public override string? Select2Text { get => Id.ToString(); }
    }


}
