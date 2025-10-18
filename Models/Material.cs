using DocumentFormat.OpenXml.Math;
using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Material:BaseDBModel
    {
        public string? MaterialID { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Unit { get; set; }
        public float? UnitCost { get; set; }

    }
}
