namespace ViewModels.CRUD.Interfaces
{
    public interface ICraftMultiSelect
    {
        List<long> CraftIds { get; set; }
        List<CraftSkillBriefViewModel> Crafts { get; set; }
    }
}
