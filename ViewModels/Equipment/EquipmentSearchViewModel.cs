using Enums;
using Pagination;
using ViewModels.CRUD.Interfaces;

namespace ViewModels
{
    public class EquipmentSearchViewModel : BaseSearchModel, ISaveSearch
    {
        public CategoryBriefViewModel Category { get; set; } = new(false,"");
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new(false,"");
        public UOMBriefViewModel UOM { get; set; } = new(false,"");
        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; }
        public string SearchView { get; set; }
    }
}
