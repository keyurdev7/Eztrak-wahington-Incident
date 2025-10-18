using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels
{
    public class EquipmentFieldsModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        public long Id { get; set; }
        public string? EquipmentFieldsID { get; set; }
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        public string? Category { get; set; }
        public double HourlyRate { get; set; }
    }
}
