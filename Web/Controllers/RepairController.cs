using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class RepairController : CrudBaseController<RepairModifyViewModel, RepairModifyViewModel, RepairDetailViewModel, RepairDetailViewModel, RepairBriefViewModel, RepairSearchViewModel>
    {
        public RepairController(IRepairService<RepairModifyViewModel, RepairModifyViewModel, RepairDetailViewModel> service, ILogger<RepairController> logger, IMapper mapper) : base(service, logger, mapper, "Repair", "Repair", false)
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
    }
}

