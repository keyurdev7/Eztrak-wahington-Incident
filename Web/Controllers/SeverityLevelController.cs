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
