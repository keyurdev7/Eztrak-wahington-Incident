using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.CRUD.Interfaces
{
    public interface ILastUpdatedBy
    {
        string LastUpdatedBy { get; set; }
        DateTime LastUpdatedOn { get; set; }
    }
}
