using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class SourceController : CrudBaseController<SourceModifyViewModel, SourceModifyViewModel, SourceDetailViewModel, SourceDetailViewModel, SourceBriefViewModel, SourceSearchViewModel>
    {
        public SourceController(ISourceService<SourceModifyViewModel, SourceModifyViewModel, SourceDetailViewModel> service, ILogger<SourceController> logger, IMapper mapper) : base(service, logger, mapper, "Source", "Source", false)
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

