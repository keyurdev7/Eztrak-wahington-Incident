using Enums;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class TaskTypeDetailViewModel : BaseCrudViewModel, ISelect2BaseVM
    {
        public long? Id { get; set; }
        public string Select2Text { get => Code.ToString(); }
        public string Code { get; set; }
        public string Title { get; set; }
        public TaskCatalog Category { get; set; }
        [Display(Name = "Budget Hours")]
        public double BudgetHours { get; set; }
        [Display(Name = "Budget Cost")]
        public double BudgetCost { get; set; }
        public double Labor { get; set; }
        public double Material { get; set; }
        public double Equipment { get; set; }
        public List<TaskLaborViewModel> TaskLabors { get; set; } = new();
        public List<TaskMaterialViewModel> TaskMaterials { get; set; } = new();
        public List<TaskEquipmentViewModel> TaskEquipments { get; set; } = new();
        public List<TaskWorkStepViewModel> TaskWorkSteps { get; set; } = new();

    }
}
