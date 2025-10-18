using Enums;
using ViewModels.CRUD;
using ViewModels.Shared;
using ViewModels.Shared.Notes;

namespace ViewModels
{
    public class AssetTabsModifyViewModel 
    {
        public CrudUpdateViewModel CrudUpdateViewModel { get; set; }
        public List<INotesViewModel> Notes { get; set; }
        public List<WorkOrderDetailViewModel> ApprovalWorkOrderHistory { get; set; }

        public List<ConditionDetailViewModel> Conditions { get; set; }
    }
}
