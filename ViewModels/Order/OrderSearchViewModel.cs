using Enums;
using Pagination;
using ViewModels.Manager;

namespace ViewModels
{
    public class OrderSearchViewModel : BaseSearchModel
    {
        public OrderSearchViewModel()
        {
            OrderByColumn = "Id";
            OrderDir = PaginationOrderCatalog.Desc;
        }

        public OrderTypeCatalog Type { get; set; }
        public InventoryBriefViewModel Inventory { get; set; } = new();
        public WorkOrderBriefViewModel WorkOrder { get; set; } = new();
    }
}
