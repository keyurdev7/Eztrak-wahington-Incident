using Enums;
using Pagination;
using ViewModels.CRUD.Interfaces;

namespace ViewModels
{
    public class AssetSearchViewModel : BaseSearchModel, ISaveSearch
    {
        public long? Id { get; set; }
        public bool ForMapData { get; set; } = false;
        public AssetTypeBriefViewModel AssetType { get; set; } = new(false,"");
        public ManufacturerBriefViewModel Manufacturer { get; set; } = new(false, "");
        public ConditionBriefViewModel Condition { get; set; } = new(false, "");
        public MUTCDBriefViewModel MUTCD { get; set; } = new(false, "");

        public List<long> AssetTypeLevel2 { get; set; } = new();
        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; } 
        public string SearchView { get; set; }
        public bool ShowOutOfServiceAssets { get; set; }

    }

}
