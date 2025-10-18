using Enums;
using Models;
using Pagination;
using ViewModels.CRUD.Interfaces;

namespace ViewModels
{
    public class TransactionSearchViewModel : BaseSearchModel, ISaveSearch
    {
        public SourceBriefViewModel Source { get; set; } = new();
        public SupplierBriefViewModel Supplier { get; set; } = new(false, "");
        public LocationBriefViewModel Location { get; set; } = new(false, "");
        public UOMBriefViewModel UOM { get; set; } = new(false, "");
        public InventoryBriefViewModel Inventory { get; set; } = new();
        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; }
        public string SearchView { get; set; }
    }
}
