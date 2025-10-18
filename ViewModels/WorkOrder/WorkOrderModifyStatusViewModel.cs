using System.ComponentModel.DataAnnotations;
using Enums;
using Helpers.Extensions;

namespace ViewModels
{
    public class WorkOrderModifyStatusViewModel : WorkOrderDetailViewModel
    {
        [Required]
        public new WOStatusCatalog Status { get; set; }
        public string FormattedStatus
        {
            get
            {
                return Status.GetDisplayName();
            }
        }
        public string? Comment { get; set; }
    }
}
