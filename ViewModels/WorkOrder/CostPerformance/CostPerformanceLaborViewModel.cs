namespace ViewModels.WorkOrder.CostPerformance
{
    public class CostPerformanceLaborViewModel
    {
        public CraftSkillBriefViewModel CraftSkill { get; set; } = new();
        public double Hours { get; set; }
        public double ActualHours { get; set; }
        public double Cost { get; set; }
        public double ActualCost { get; set; }
    }
}
