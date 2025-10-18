using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.WorkOrder
{
    public class WOAssetViewModel
    {
        public long? Id { get; set; }
        [Display(Name = "Asset ID", Prompt = "Asset Id")]
        public string? SystemGeneratedId { get; set; }
        public string FormattedSystemGeneratedId { get => !string.IsNullOrEmpty(SystemGeneratedId) ? SystemGeneratedId.ToString() : "-"; }
        public string? Description { get; set; }
        public string? Street { get; set; }
        [Display(Name = "Asset Type", Prompt = "Asset Type")]
        public string? AssetType { get; set; }
        public long? AssetTypeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
