using Enums;
using Pagination;
using ViewModels.CRUD.Interfaces;

namespace ViewModels
{
    public class InventorySearchViewModel : BaseSearchModel, ISaveSearch
    {
        public bool ShowZeroQuantityItems { get; set; }
        public CategoryBriefViewModel Category { get; set; } = new(false, "");
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new(false, "");
        public UOMBriefViewModel UOM { get; set; } = new(false, "");
        public MUTCDBriefViewModel MUTCD { get; set; } = new(false, "");
        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; } = SearchFilterTypeCatalog.MaterialCostReport;
        public string SearchView { get; set; }
    }
}
