using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enums
{
    public enum SSRStatus
    {
        Open,
        [Display(Name = "WO Created")]
        WOCreated,
        //Completed,
        Dismissed
    }
}
