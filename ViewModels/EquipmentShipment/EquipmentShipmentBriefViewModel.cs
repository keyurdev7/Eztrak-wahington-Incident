using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EquipmentShipmentBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public EquipmentShipmentBriefViewModel() : base(true, "The Shipment field is required.")
        {

        }
        [DisplayName("EquipmentShipment")]
        public string Name { get; set; }

        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
    }
}