using DocumentFormat.OpenXml.Drawing.Diagrams;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class InventoryBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public InventoryBriefViewModel() : base(false, "")
        {

        }
        [DisplayName("Inventory")]
        public string? Name { get => Description + " - " + SystemGeneratedId?.ToString(); }

        [DisplayName("Material ID")]

        public string? ItemNo { get; set; }
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