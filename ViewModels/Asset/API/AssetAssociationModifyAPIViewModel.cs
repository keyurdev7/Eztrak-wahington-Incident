using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class AssetAssociationModifyAPIViewModel
    {
        public long ConditionId { get; set; }
        public List<AssetAssociationDetailAPIViewModel> AssetAssociations { get; set; } = new();
    }
}
