using Enums;
using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels.WorkOrderTasks
{
    public class WorkOrderTasksModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Task Description", Prompt = "Task Description")]
        public string TaskDescription { get; set; }
        [Display(Name = "Task Due Date", Prompt = "Task Due Date")]
        public DateTime TaskDueDate { get; set; } = DateTime.Now;
        public WOTaskStatusCatalog Status { get; set; }
        public long WorkOrderId { get; set; }
    }
}
