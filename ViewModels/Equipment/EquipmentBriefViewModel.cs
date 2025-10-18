using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.ComponentModel;

namespace ViewModels
{
    public class EquipmentBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public EquipmentBriefViewModel() : base(false, "")
        {

        }
        [DisplayName("Equipment")]
        public string? Name { get => Description + " - " + SystemGeneratedId?.ToString(); }
        [DisplayName("Equipment ID")]

        public string? SystemGeneratedId { get; set; }
        public string? Description { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Description + " - " + SystemGeneratedId?.ToString();
            }
        }
    }
}