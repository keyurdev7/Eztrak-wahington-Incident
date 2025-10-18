using Helpers.Extensions;
using System.ComponentModel;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetTypeLevel1TreeViewModel 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<AssetTypeLevel2TreeViewModel> AssetTypeLevel2 { get; set; } = new();
    }
}
