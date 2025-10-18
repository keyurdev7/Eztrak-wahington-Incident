using Pagination;
using System.ComponentModel;

namespace ViewModels.Timesheet
{
    public class PayPeriodBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public PayPeriodBriefViewModel() : base(true, "The Pay Period field is required.")
        {

        }

        public PayPeriodBriefViewModel(bool isValidationEnabled) : base(isValidationEnabled, "The Pay Period field is required.")
        {

        }
        public PayPeriodBriefViewModel(bool isValidationEnabled, string errorMessage) : base(isValidationEnabled, errorMessage)
        {

        }
        [DisplayName("Pay Period")]
        public override string? Select2Text { get; set; }
    }

    public class PayPeriodSearchViewModel : BaseSearchModel
    {
    }
}
