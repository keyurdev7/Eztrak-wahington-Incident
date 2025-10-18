namespace ViewModels
{
    public class MaintenanceReportViewModel : AssetDetailViewModel
    {
        public int MonthsRemainingInNextMaintenance
        {
            get
            {
                DateTime now = DateTime.Now;
                if (NextMaintenanceDate <= now)
                {
                    return 0;
                }
                else
                {
                    int monthsDifference = ((NextMaintenanceDate.Year - now.Year) * 12) + NextMaintenanceDate.Month - now.Month;
                    return monthsDifference;
                }
            }
        }

        public string FormattedMonthsRemaining
        {
            get
            {
                var monthsRemaining = MonthsRemainingInNextMaintenance;
                if (monthsRemaining <= 1)
                {
                    return "1 month";
                }
                else if (monthsRemaining <= 3)
                {
                    return "3 months";
                }
                else if (monthsRemaining <= 6)
                {
                    return "6 months";
                }
                else
                {
                    return $"{monthsRemaining} months";
                }
            }
        }
        public bool HideCreate
        {
            get
            {
                return MonthsRemainingInNextMaintenance > 3 ? true : false;

            }
        }
    }
}
