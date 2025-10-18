using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class RelationshipController : CrudBaseController<RelationshipModifyViewModel, RelationshipModifyViewModel, RelationshipDetailViewModel, RelationshipDetailViewModel, RelationshipBriefViewModel, RelationshipSearchViewModel>
    {
        public RelationshipController(
            IRelationshipService<RelationshipModifyViewModel, RelationshipModifyViewModel, RelationshipDetailViewModel> service,
            ILogger<RelationshipController> logger,
            IMapper mapper
        ) : base(service, logger, mapper, "Relationship", "Source", false)
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel { title = "Name", data = "Name", orderable = true },
                new DataTableViewModel { title = "Action", data = null, className = "action text-right exclude-form-export" }
            };
        }
    }
}
