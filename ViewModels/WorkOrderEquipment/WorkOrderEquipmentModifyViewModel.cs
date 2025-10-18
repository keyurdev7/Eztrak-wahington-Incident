using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels
{
    public class WorkOrderEquipmentModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Quantity")]
        [Range(1, double.MaxValue, ErrorMessage = "The field Quantity must be greater than zero.")]
        [Required]
        public double Quantity { get; set; }

        [Display(Name = "Hours")]
        [Range(1, double.MaxValue, ErrorMessage = "The field Hours must be greater than zero.")]
        [Required]
        public double Hours { get; set; }

        public WorkOrderBriefViewModel WorkOrder { get; set; } = new();
        public EquipmentDetailViewModel Equipment { get; set; } = new();
    }
}
