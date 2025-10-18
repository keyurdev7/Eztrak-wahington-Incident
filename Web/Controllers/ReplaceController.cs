using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class ReplaceController : CrudBaseController<ReplaceModifyViewModel, ReplaceModifyViewModel, ReplaceDetailViewModel, ReplaceDetailViewModel, ReplaceBriefViewModel, ReplaceSearchViewModel>
    {
        public ReplaceController(IReplaceService<ReplaceModifyViewModel, ReplaceModifyViewModel, ReplaceDetailViewModel> service, ILogger<ReplaceController> logger, IMapper mapper) : base(service, logger, mapper, "Replace", "Replace", false)
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

