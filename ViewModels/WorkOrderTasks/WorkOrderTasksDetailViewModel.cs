using Enums;
using Helpers.Datetime;
using Models.Common.Interfaces;
using Pagination;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class WorkOrderTasksDetailViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Prompt = "Task Description")]
        public string TaskDescription { get; set; }
        public DateTime TaskDueDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn.FormatDate();
            }
        }
        public string FormattedTaskDueDate
        {
            get
            {
                return TaskDueDate.FormatDate();
            }
        }
        public long WorkOrderId { get; set; }
        public WOTaskStatusCatalog Status { get; set; }

    }

    public class WorkOrderTasksIndexViewModel
    {
        public List<WorkOrderTasksDetailViewModel> CompletedTasks { get; set; } = new();
        public List<WorkOrderTasksDetailViewModel> UnCompletedTasks { get; set; } = new();
    }

    public class WorkOrderTasksSearchViewModel : BaseSearchModel
    {
        public DateTime TaskDueDate { get; set; }
    }
}
