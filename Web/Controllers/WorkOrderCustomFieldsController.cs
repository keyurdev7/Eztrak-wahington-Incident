using AutoMapper;
using Enums;
using Repositories.Common;
using ViewModels;

namespace Web.Controllers
{
    public class WorkOrderCustomFieldsController : DynamicColumnController
    {
        public WorkOrderCustomFieldsController(IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnDetailViewModel> service, ILogger<DynamicColumnController> logger, IMapper mapper) : base(service, logger, mapper, "WorkOrderCustomFields", "Work Order Custom Fields", DynamicColumnEntityType.WorkOrder)
        {
        }
    }
}

