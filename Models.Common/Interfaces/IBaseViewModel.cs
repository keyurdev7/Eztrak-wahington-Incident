using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common.Interfaces
{
    public interface IBaseViewModel
    {
        bool IsDeleted { get; set; }
        bool IsActive { get; set; }
        long CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        long UpdatedBy { get; set; }
        DateTime UpdatedOn { get; set; }
    }
    
}
