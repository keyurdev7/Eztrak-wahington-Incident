using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Pagination;
using Repositories.Common;
using ViewModels;
using ViewModels.CRUD;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class MUTCDController : CrudBaseController<MUTCDModifyViewModel, MUTCDModifyViewModel, MUTCDDetailViewModel, MUTCDDetailViewModel, MUTCDBriefViewModel, MUTCDSearchViewModel>
    {
        private readonly IMUTCDService<MUTCDModifyViewModel, MUTCDModifyViewModel, MUTCDDetailViewModel> _service;

        public MUTCDController(IMUTCDService<MUTCDModifyViewModel, MUTCDModifyViewModel, MUTCDDetailViewModel> service, ILogger<MUTCDController> logger, IMapper mapper) : base(service, logger, mapper, "MUTCD", "MUTCD")
        {
            _service = service;
        }
        public override async Task<ActionResult> Index()
        {
            var vm = new CrudListViewModel();
            vm.Title = "MUTCD";
            vm.DisableSearch = false;
            vm.Filters = new MUTCDSearchViewModel();
            vm.SearchViewPath = "~/Views/MUTCD/_Search.cshtml";
            return await Task.FromResult(View(vm));
        }

        public override async Task<IActionResult> Search(MUTCDSearchViewModel searchModel)
        {
            searchModel.DisablePagination = true;
            var response = await _service.GetAll<MUTCDDetailViewModel>(searchModel);
            var paginatedResult = response as RepositoryResponseWithModel<PaginatedResultModel<MUTCDDetailViewModel>>;
            var vm = new DataCardViewModel<MUTCDDetailViewModel>();
            vm.PaginatedResult = paginatedResult?.ReturnModel;
            vm.CurrentPage = searchModel.CurrentPage;
            vm.RecordsPerPage = searchModel.PerPage;
            vm.DisablePagination = searchModel.DisablePagination;
            return View("~/Views/MUTCD/_Datacards.cshtml", vm);
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>();
            //return new List<DataTableViewModel>()
            //{
            //    new DataTableViewModel{title = "Code",data = "Code"},
            //    new DataTableViewModel{title = "Description",data = "Description"},
            //    new DataTableViewModel{title = "Image",data = "FormattedImageUrl", format = "html", formatValue="image", className = "image-thumbnail action"},

            //    new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            //};
        }
        public override Task<JsonResult> Select2(string prefix, int pageSize, int pageNumber, string customParams)
        {
            return base.Select2(prefix, pageSize, pageNumber, customParams);
        }

    }
}

