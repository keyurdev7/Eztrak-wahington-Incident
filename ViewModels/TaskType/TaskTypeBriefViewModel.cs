using DocumentFormat.OpenXml.Wordprocessing;
using Enums;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class TaskTypeBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public string? Code { get; set; }
        public string? Title { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Title;
            }
        }
        public TaskCatalog Category { get; set; }
        [Display(Name = "Budget Hours")]
        public double? BudgetHours { get; set; }
        [Display(Name = "Budget Cost")]
        public double? BudgetCost { get; set; }
        public double Hours { get; set; }
        public double Labor { get; set; }
        public double Material { get; set; }
        public double Equipment { get; set; }
        public double Budget { get => Labor + Material + Equipment; }
        public TaskTypeBriefViewModel() : base(true, "Add Work Step")
        {

        }
    }

    public class TaskTypeForIssueBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public TaskTypeForIssueBriefViewModel() : base(true, "Task is required")
        {

        }
    }

}
