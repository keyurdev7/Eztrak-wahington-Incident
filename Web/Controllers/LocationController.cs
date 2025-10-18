using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class LocationController : CrudBaseController<LocationModifyViewModel, LocationModifyViewModel, LocationDetailViewModel, LocationDetailViewModel, LocationBriefViewModel, LocationSearchViewModel>
    {
        public LocationController(ILocationService<LocationModifyViewModel, LocationModifyViewModel, LocationDetailViewModel> service, ILogger<LocationController> logger, IMapper mapper) : base(service, logger, mapper, "Location", "Location", false)
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

