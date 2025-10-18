using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class SupplierController : CrudBaseController<SupplierModifyViewModel, SupplierModifyViewModel, SupplierDetailViewModel, SupplierDetailViewModel, SupplierBriefViewModel, SupplierSearchViewModel>
    {
        public SupplierController(ISupplierService<SupplierModifyViewModel, SupplierModifyViewModel, SupplierDetailViewModel> service, ILogger<SupplierController> logger, IMapper mapper) : base(service, logger, mapper, "Supplier", "Supplier", false)
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

