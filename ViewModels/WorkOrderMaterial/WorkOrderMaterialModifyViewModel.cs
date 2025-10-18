using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels
{
    public class WorkOrderMaterialModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Quantity")]
        [Range(1, double.MaxValue, ErrorMessage = "The field Quantity must be greater than zero.")]
        [Required]
        public double Quantity { get; set; }

        public WorkOrderBriefViewModel WorkOrder { get; set; } = new();
        public InventoryBriefViewModel Inventory { get; set; } = new();
    }
}
