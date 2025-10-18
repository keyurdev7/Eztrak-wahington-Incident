using AutoMapper;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class AssignResponseTeamsController : CrudBaseController<
        AssignResponseTeamsModifyViewModel,
        AssignResponseTeamsModifyViewModel,
        AssignResponseTeamsDetailViewModel,
        AssignResponseTeamsDetailViewModel,
        AssignResponseTeamsBriefViewModel,
        AssignResponseTeamsSearchViewModel>
    {
        public AssignResponseTeamsController(
            IAssignResponseTeamsService<AssignResponseTeamsModifyViewModel, AssignResponseTeamsModifyViewModel, AssignResponseTeamsDetailViewModel> service,
            ILogger<AssignResponseTeamsController> logger,
            IMapper mapper)
            : base(service, logger, mapper, "AssignResponseTeams", "Assign Response Teams", false)
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