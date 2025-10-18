using System;
using ViewModels.Shared;

namespace ViewModels.Report.RawReport
{
    public class WorkOrderRawReportColumns
    {
        public List<BaseMinimalVM> EquipmentColumns { get; set; }
        public List<BaseMinimalVM> MaterialColumns { get; set; }
        public List<BaseMinimalVM> TechnicianColumns { get; set; }
    }
}

