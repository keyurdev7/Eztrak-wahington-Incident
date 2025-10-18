using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class StatusLegendController : CrudBaseController<StatusLegendModifyViewModel, StatusLegendModifyViewModel, StatusLegendDetailViewModel, StatusLegendDetailViewModel, StatusLegendBriefViewModel, StatusLegendSearchViewModel>
    {
        public StatusLegendController(
            IStatusLegendService<StatusLegendModifyViewModel, StatusLegendModifyViewModel, StatusLegendDetailViewModel> service,
            ILogger<StatusLegendController> logger,
            IMapper mapper
        ) : base(service, logger, mapper, "StatusLegend", "Status Legend", false)
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel { title = "Name", data = "Name", orderable = true },
                new DataTableViewModel { title = "Color", data = "Color", orderable = true },
                new DataTableViewModel { title = "Action", data = null, className = "action text-right exclude-form-export" }
            };
        }
    }
}
