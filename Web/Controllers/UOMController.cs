using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class UOMController : CrudBaseController<UOMModifyViewModel, UOMModifyViewModel, UOMDetailViewModel, UOMDetailViewModel, UOMBriefViewModel, UOMSearchViewModel>
    {
        public UOMController(IUOMService<UOMModifyViewModel, UOMModifyViewModel, UOMDetailViewModel> service, ILogger<UOMController> logger, IMapper mapper) : base(service, logger, mapper, "UOM", "UOM", false)
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
    }
}

