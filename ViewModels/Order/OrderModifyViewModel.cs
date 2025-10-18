using Enums;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels
{
    public class OrderModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {

        public OrderStatus Status { get; set; }

        public string OrderNumber { get; set; }

        public string? Notes { get; set; }

        public WorkOrderForIssueBriefViewModel WorkOrder { get; set; } = new();

        public List<OrderItemModifyViewModel> OrderItems { get; set; } = new();

        public OrderTypeCatalog Type { get; set; }
    }

}

