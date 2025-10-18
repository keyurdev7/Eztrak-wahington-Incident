using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.CRUD.Interfaces
{
    public interface ISaveSearch
    {
        UserSearchSettingBriefViewModel UserSearchSetting { get; set; }
        SearchFilterTypeCatalog? Type { get; set; }
        string SearchView { get; set; }
    }
}
