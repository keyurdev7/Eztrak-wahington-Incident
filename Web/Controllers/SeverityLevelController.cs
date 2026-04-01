using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class SeverityLevelController : CrudBaseController<
        SeverityLevelModifyViewModel,
        SeverityLevelModifyViewModel,
        SeverityLevelDetailViewModel,
        SeverityLevelDetailViewModel,
        SeverityLevelBriefViewModel,
        SeverityLevelSearchViewModel>
    {
        public SeverityLevelController(
            ISeverityLevelService<
                SeverityLevelModifyViewModel,
                SeverityLevelModifyViewModel,
                SeverityLevelDetailViewModel> service,
            ILogger<SeverityLevelController> logger,
            IMapper mapper
        ) : base(service, logger, mapper, "SeverityLevel", "Severity Level", false)
        {
        }

        private const string FixedSeverityMessage =
            "Severity levels are fixed and cannot be managed from this screen.";

        public override Task<ActionResult> Index() =>
            Task.FromResult<ActionResult>(NotFound(FixedSeverityMessage));

        public override Task<IActionResult> Search(SeverityLevelSearchViewModel searchModel) =>
            Task.FromResult<IActionResult>(NotFound(FixedSeverityMessage));

        public override Task<ActionResult> Create() =>
            Task.FromResult<ActionResult>(NotFound(FixedSeverityMessage));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override Task<ActionResult> Create(SeverityLevelModifyViewModel model) =>
            Task.FromResult<ActionResult>(NotFound(FixedSeverityMessage));

        public override Task<ActionResult> Update(int id) =>
            Task.FromResult<ActionResult>(NotFound(FixedSeverityMessage));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override Task<ActionResult> Update(SeverityLevelModifyViewModel model) =>
            Task.FromResult<ActionResult>(NotFound(FixedSeverityMessage));

        public override Task<ActionResult> Detail(int id) =>
            Task.FromResult<ActionResult>(NotFound(FixedSeverityMessage));

        public override Task<ActionResult> Delete(int id) =>
            Task.FromResult<ActionResult>(NotFound(FixedSeverityMessage));

        public override Task<JsonResult> Select2(string prefix, int pageSize, int pageNumber, string customParams) =>
            Task.FromResult(Json(new { results = Array.Empty<object>(), pagination = new { more = false } }));

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel { title = "Name", data = "Name", orderable = true },
                new DataTableViewModel { title = "Description", data = "Description", orderable = true },
                new DataTableViewModel { title = "Color", data = "Color", orderable = true },
                new DataTableViewModel { title = "Action", data = null, className = "action text-right exclude-form-export" }
            };
        }
    }
}
