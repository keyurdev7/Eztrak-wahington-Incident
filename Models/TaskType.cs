using Enums;
using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TaskType : BaseDBModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public TaskCatalog Category { get; set; }
        public double BudgetHours { get; set; }
        public double BudgetCost { get; set; }
        public double Labor { get; set; }
        public double Material { get; set; }
        public double Equipment { get; set; }
        public List<TaskWorkStep>? TaskWorkSteps { get; set; }
        public List<TaskMaterial>? TaskMaterials { get; set; }
        public List<TaskEquipment>? TaskEquipment { get; set; }
        public List<TaskLabor>? TaskLabors { get; set; }
    }
}
