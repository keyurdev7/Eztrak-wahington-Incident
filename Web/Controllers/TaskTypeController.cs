using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using ViewModels.WorkOrderTechnician;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class TaskTypeController : CrudBaseController<TaskTypeModifyViewModel, TaskTypeModifyViewModel, TaskTypeDetailViewModel, TaskTypeDetailViewModel, TaskTypeBriefViewModel, TaskTypeSearchViewModel>
    {
        public TaskTypeController(ITaskTypeService<TaskTypeModifyViewModel, TaskTypeModifyViewModel, TaskTypeDetailViewModel> service, ILogger<TaskTypeController> logger, IMapper mapper) : base(service, logger, mapper, "TaskType", "Work Step", false)
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Code",data = "Code", orderable = true},
                new DataTableViewModel{title = "Category",data = "Category", orderable = true},
                new DataTableViewModel{title = "Title",data = "Title", orderable = true},
                new DataTableViewModel{title = "Labor",data = "Labor", className="dt-currency", orderable = true},
                new DataTableViewModel{title = "Equipment",data = "Equipment", className="dt-currency", orderable = true},
                new DataTableViewModel{title = "Material",data = "Material", className="dt-currency", orderable = true},
                new DataTableViewModel{title = "Total",data = "BudgetCost", className="dt-currency", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        protected override TaskTypeSearchViewModel SetSelect2CustomParams(string customParams)
        {
            var svm = JsonConvert.DeserializeObject<TaskTypeSearchViewModel>(customParams);
            return svm;
        }
        public override Task<ActionResult> Create(TaskTypeModifyViewModel model)
        {
            ValidateRecords(model);
            return base.Create(model);
        }
        public override Task<ActionResult> Update(TaskTypeModifyViewModel model)
        {
            ValidateRecords(model);
            return base.Update(model);
        }
        public IActionResult _LaborRow(int rowNumber)
        {
            ViewData["RowNumber"] = rowNumber;
            var model = new TaskLaborViewModel();
            return PartialView("_LaborRow", model);
        }

        public IActionResult _MaterialRow(int rowNumber)
        {
            ViewData["RowNumber"] = rowNumber;
            var model = new TaskMaterialViewModel();
            return PartialView("_MaterialRow", model);
        }

        public IActionResult _EquipmentRow(int rowNumber)
        {
            ViewData["RowNumber"] = rowNumber;
            var model = new TaskEquipmentViewModel();
            return PartialView("_EquipmentRow", model);
        }

        private void ValidateRecords(TaskTypeModifyViewModel model)
        {
            var invalidTaskWorkSteps = model.TaskWorkSteps.Where(x => string.IsNullOrEmpty(x.Title)).Count() > 0;
            if (invalidTaskWorkSteps)
            {
                ModelState.AddModelError("", "Please provide titles for all the work order steps");
            }
        }
    }
}

