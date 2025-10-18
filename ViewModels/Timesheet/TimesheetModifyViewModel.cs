using ViewModels.Shared;
using Models.Common.Interfaces;
using ViewModels.CRUD.Interfaces;
using ViewModels.Technician;
using ViewModels.Manager;

namespace ViewModels.Timesheet
{
    public class TimesheetModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier, ILastUpdatedBy
    {
        public long CraftId { get; set; }
        public long WorkOrderTechnicianId { get; set; }
        public float STHours { get; set; }
        public float OTHours { get; set; }
        public float DTHours { get; set; }
        public double TotalCost { get; set; }

        public DateTime WeekEnding { get; set; } = DateTime.Now;
        public TechnicianBriefViewModel Technician { get; set; } = new();
        public ManagerBriefViewModel Approver { get; set; } = new();
        public WorkOrderBriefViewModel WorkOrder { get; set; } = new();
        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedOn { get; set; }
    }
}
