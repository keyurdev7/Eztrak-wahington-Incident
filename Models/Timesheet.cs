using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Timesheet : BaseDBModel
    {
        public TimesheetApproveStatus ApproveStatus { get; set; }
        public DateTime WeekEnding { get; set; }
        public DateTime DueDate { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }
        public double PerDiem { get; set; }
        public double DailyPerDiem { get; set; }
        public long WorkOrderId { get; set; }
        public double Material { get; set; }
        public double Equipment { get; set; }
        public double Other { get; set; }
        public double Airfare { get; set; }
        public double Miles { get; set; }
        public double TotalCost { get; set; }

        public string? Note { get; set; }
        public PaymentIndicatorStatus? PaymentIndicator { get; set; }

        [ForeignKey("WorkOrderTechnician")]
        public long? WorkOrderTechnicianId { get; set; }
        public WorkOrderTechnician WorkOrderTechnician { get; set; }

        [ForeignKey("Approver")]
        public long? ApproverId { get; set; }
        public ApplicationUser? Approver { get; set; }
        public List<TimesheetBreakdown>? TimesheetBreakdowns { get; set; } = new List<TimesheetBreakdown>();

    }
}
