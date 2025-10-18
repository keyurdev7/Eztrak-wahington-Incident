using AutoMapper;
using Repositories.Common;
using ViewModels.DataTable;
using ViewModels;
using Web.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ShipmentController : CrudBaseController<ShipmentModifyViewModel, ShipmentModifyViewModel, ShipmentDetailViewModel, ShipmentDetailViewModel, ShipmentBriefViewModel, ShipmentSearchViewModel>
    {
        public ShipmentController(IShipmentService<ShipmentModifyViewModel, ShipmentModifyViewModel, ShipmentDetailViewModel> service, ILogger<ShipmentController> logger, IMapper mapper) : base(service, logger, mapper, "Shipment", "Shipment")
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Supplier",data = "Supplier.Name"},
                new DataTableViewModel{title = "OH Quantity",data = "OhQuantity"},
                new DataTableViewModel{title = "Source",data = "Source.Name"},
                new DataTableViewModel{title = "UOM",data = "UOM.Name"},
                
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        public ActionResult AddShipments(long Id)
        {
            var shipment = new ShipmentModifyViewModel() { InventoryId = Id };
            return UpdateView(GetUpdateViewModel("Create", shipment));
        }
    }
}

