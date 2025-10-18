using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class MountTypeController : CrudBaseController<MountTypeModifyViewModel, MountTypeModifyViewModel, MountTypeDetailViewModel, MountTypeDetailViewModel, MountTypeBriefViewModel, MountTypeSearchViewModel>
    {
        public MountTypeController(IMountTypeService<MountTypeModifyViewModel, MountTypeModifyViewModel, MountTypeDetailViewModel> service, ILogger<MountTypeController> logger, IMapper mapper) : base(service, logger, mapper, "MountType", "MountType", false)
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

