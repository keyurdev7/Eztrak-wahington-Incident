using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class ManufacturerController : CrudBaseController<ManufacturerModifyViewModel, ManufacturerModifyViewModel, ManufacturerDetailViewModel, ManufacturerDetailViewModel, ManufacturerBriefViewModel, ManufacturerSearchViewModel>
    {
        public ManufacturerController(IManufacturerService<ManufacturerModifyViewModel, ManufacturerModifyViewModel, ManufacturerDetailViewModel> service, ILogger<ManufacturerController> logger, IMapper mapper) : base(service, logger, mapper, "Manufacturer", "Manufacturer", false)
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

