using Enums;
using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.CRUD.Interfaces;
using ViewModels.Shared;

namespace ViewModels
{
    public class TaskTypeModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Code", Prompt = "Code")]
        public string? Code { get; set; }
        [Display(Name = "Title", Prompt = "Title")]
        public string Title { get; set; }
        [Required]
        public TaskCatalog? Category { get; set; }
        [Display(Name = "Budget Hours", Prompt = "Budget Hours")]
        public double BudgetHours { get; set; }
        [Display(Name = "Budget Cost", Prompt = "Budget Cost")]
        public double BudgetCost { get; set; }
        public double? Labor { get; set; }
        public double? Material { get; set; }
        public double? Equipment { get; set; }
        public List<TaskLaborViewModel> TaskLabors { get; set; } = new();
        public List<TaskMaterialViewModel> TaskMaterials { get; set; } = new();
        public List<TaskEquipmentViewModel> TaskEquipments { get; set; } = new();
        public List<TaskWorkStepViewModel> TaskWorkSteps { get; set; } = new();
    }
}
