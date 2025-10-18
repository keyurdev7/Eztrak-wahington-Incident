using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class ConditionController : CrudBaseController<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel, ConditionDetailViewModel, ConditionBriefViewModel, ConditionSearchViewModel>
    {
        public ConditionController(IConditionService<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel> service, ILogger<ConditionController> logger, IMapper mapper) : base(service, logger, mapper, "Condition", "Condition")
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                 new DataTableViewModel{title = "Color",data = "Color"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
    }
}

