using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class EventTypeController : CrudBaseController<EventTypeModifyViewModel, EventTypeModifyViewModel, EventTypeDetailViewModel, EventTypeDetailViewModel, EventTypeBriefViewModel, EventTypeSearchViewModel>
    {
        public EventTypeController(IEventTypeService<EventTypeModifyViewModel, EventTypeModifyViewModel, EventTypeDetailViewModel> service, ILogger<EventTypeController> logger, IMapper mapper)
            : base(service, logger, mapper, "EventType", "EventType", false)
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel { title = "Name", data = "Name", orderable = true },
                new DataTableViewModel { title = "Description", data = "Description", orderable = true },
                new DataTableViewModel { title = "Action", data = null, className = "action text-right exclude-form-export" }
            };
        }
    }
}
