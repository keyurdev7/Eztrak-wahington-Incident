using AutoMapper;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class DynamicColumnController : CrudBaseController<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnDetailViewModel, DynamicColumnDetailViewModel, DynamicColumnBriefViewModel, DynamicColumnSearchViewModel>
    {
        private readonly DynamicColumnEntityType _type;

        public DynamicColumnController(IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnDetailViewModel> service, ILogger<DynamicColumnController> logger, IMapper mapper, string controllerName, string title, DynamicColumnEntityType type) : base(service, logger, mapper, controllerName, title)
        {
            _type = type;
        }

        protected override DynamicColumnSearchViewModel SetDefaultSearchViewModel()
        {
            var model = new DynamicColumnSearchViewModel();
            model.EntityType = _type; 
            return model;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name", orderable = true},
                new DataTableViewModel{title = "Title",data = "Title", orderable = true},
                new DataTableViewModel{title = "Type",data = "FormattedType", orderable = true, sortingColumn = "Type"},
                //new DataTableViewModel{title = "Table",data = "FormattedEntityType"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        public override Task<ActionResult> Create(DynamicColumnModifyViewModel model)
        {
            model.EntityType = _type;
            return base.Create(model);
        }

        public override Task<ActionResult> Update(DynamicColumnModifyViewModel model)
        {
            model.EntityType = _type;
            return base.Update(model);
        }
    }
}

