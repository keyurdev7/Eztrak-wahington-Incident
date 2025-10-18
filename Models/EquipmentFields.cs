using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EquipmentFields : BaseDBModel
    {
        public string? EquipmentFieldsID { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public double HourlyRate { get; set; }
    }
}
