using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using Select2.Model;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class CraftSkillController : CrudBaseController<CraftSkillModifyViewModel, CraftSkillModifyViewModel, CraftSkillDetailViewModel, CraftSkillDetailViewModel, CraftSkillBriefViewModel, CraftSkillSearchViewModel>
    {
        private readonly ICraftSkillService<CraftSkillModifyViewModel, CraftSkillModifyViewModel, CraftSkillDetailViewModel> _service;

        public CraftSkillController(ICraftSkillService<CraftSkillModifyViewModel, CraftSkillModifyViewModel, CraftSkillDetailViewModel> service, ILogger<CraftSkillController> logger, IMapper mapper) : base(service, logger, mapper, "CraftSkill", "Craft")
        {
            _service = service;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name", orderable = true},
                new DataTableViewModel{title = "STRate",data = "STRate",className="dt-currency", orderable = true},
                new DataTableViewModel{title = "OTRate",data = "OTRate",className="dt-currency", orderable = true},
                 new DataTableViewModel{title = "DTRate",data = "DTRate",className="dt-currency", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        public async override Task<IRepositoryResponse> GetResponse(CraftSkillSearchViewModel svm)
        {
            return await _service.GetCraftSkillsForSelect2<CraftSkillBriefViewModel>(svm);
        }
    }
}

