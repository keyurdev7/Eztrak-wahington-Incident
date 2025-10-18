using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class ContractorController : CrudBaseController<ContractorModifyViewModel, ContractorModifyViewModel, ContractorDetailViewModel, ContractorDetailViewModel, ContractorBriefViewModel, ContractorSearchViewModel>
    {
        public ContractorController(IContractorService<ContractorModifyViewModel, ContractorModifyViewModel, ContractorDetailViewModel> service, ILogger<ContractorController> logger, IMapper mapper) : base(service, logger, mapper, "Contractor", "Contractor", false)
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

