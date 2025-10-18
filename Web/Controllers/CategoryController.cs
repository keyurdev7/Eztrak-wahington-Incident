using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class CategoryController : CrudBaseController<CategoryModifyViewModel, CategoryModifyViewModel, CategoryDetailViewModel, CategoryDetailViewModel, CategoryBriefViewModel, CategorySearchViewModel>
    {
        public CategoryController(ICategoryService<CategoryModifyViewModel, CategoryModifyViewModel, CategoryDetailViewModel> service, ILogger<CategoryController> logger, IMapper mapper) : base(service, logger, mapper, "Category", "Category", false)
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

