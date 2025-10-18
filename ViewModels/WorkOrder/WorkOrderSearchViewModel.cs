using Enums;
using System.ComponentModel.DataAnnotations;
using ViewModels.CRUD.Interfaces;
using ViewModels.DataTable;
using ViewModels.Manager;

namespace ViewModels
{
    public class WorkOrderSearchViewModel : BaseDateSearchModel, ISaveSearch
    {
        public ManagerBriefViewModel Manager { get; set; } = new();
        public AssetTypeBriefViewModel AssetType { get; set; } = new(false,"");
        public TaskCatalog? Task { get; set; }
        public Urgency? Urgency { get; set; }
        public WOStatusCatalog? Status { get; set; }
        public WOStatusCatalog? NotStatus { get; set; }

        [Display(Prompt = "Asset Id")]
        public string? AssetId { get; set; }
        public long? Id { get; set; }

        public UserSearchSettingBriefViewModel UserSearchSetting { get; set; } = new();
        public SearchFilterTypeCatalog? Type { get; set; }
        public string SearchView { get; set; }
    }

   
}
