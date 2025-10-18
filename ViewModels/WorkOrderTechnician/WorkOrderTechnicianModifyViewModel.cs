using Models.Common.Interfaces;
using ViewModels.Shared;

namespace ViewModels.WorkOrderTechnician
{
    public class WorkOrderTechnicianModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        public long Id { get; set; }
        public TechnicianBriefViewModel Technician { get; set; } = new();
        public CraftSkillBriefViewModel CraftSkill { get; set; } = new();
    }
}
