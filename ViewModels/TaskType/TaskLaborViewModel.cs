namespace ViewModels
{
    public class TaskLaborViewModel
    {
        public long Id { get; set; }
        public CraftSkillBriefViewModel CraftSkill { get; set; } = new();
        public double Hours { get; set; }
        public double Rate { get; set; }
        public double Total { get => Rate * Hours; }
    }
}
