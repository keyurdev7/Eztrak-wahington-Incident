using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class OrderItemModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Quantity", Prompt = "Quantity")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The order quantity must be greater than 0.")]
        public long Quantity { get; set; }

        public long OHQuantity { get; set; }

        public bool IsIssued { get; set; }

        public InventoryDetailViewModel? Inventory { get; set; } = new();
        public EquipmentDetailViewModel? Equipment { get; set; } = new();

        public OrderDetailViewModel Order { get; set; } = new();
    }

}

