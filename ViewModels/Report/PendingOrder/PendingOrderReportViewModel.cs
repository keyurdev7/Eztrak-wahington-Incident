using Helpers.Reports;
using Helpers.Reports.Interfaces;
using ViewModels.Manager;
using ViewModels.Users;

namespace ViewModels.Report.PendingOrder
{
    public class PendingOrderReportViewModel : IDate, ILabel, IIdentifier
    {

        public string Label { get; set; }
        public string Identifier { get; set; }

        public long Id { get; set; }
        public ManagerBriefViewModel? Manager { get; set; } = new();
        public ManufacturerBriefViewModel? Manufacturer { get; set; } = new();
        public double Orders { get; set; }
        public DateTime Date { get; set; }

    }

    public class PendingOrderReportGroupingKeys
    {
        public long? LocationId { get; set; }
        public long? AssignedToId { get; set; }
    }
}
