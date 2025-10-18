namespace ViewModels.WorkOrder.CostPerformance
{
    public class CostPerformanceEquipmentViewModel
    {
        public EquipmentBriefViewModel Equipment { get; set; } = new();

        public double HourlyRate { get; set; }
        public double Cost { get; set; }
        public double ActualCost { get; set; }
        public double Quantity { get; internal set; }
        public double Hours { get; internal set; }
    }
}
