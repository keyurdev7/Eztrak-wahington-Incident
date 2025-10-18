using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Condition : BaseDBModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
