using Enums;
using Helpers.Datetime;
using Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;

namespace ViewModels.WorkOrder
{
    public class WorkOrderCommentViewModel
    {
        public long? Id { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FormattedCreatedDate
        {
            get
            {
                return CreatedDate == null ? "-" : CreatedDate.Value.FormatDatetimeInPST();
            }
        }
        public string Comment { get; set; }
        public List<AttachmentVM> ImagesList { get; set; } = new();
        public WOStatusCatalog Status { get; set; }
        public string FormattedStatus
        {
            get
            {
                return Status.GetDisplayName();
            }
        }
    }
}
