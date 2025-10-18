namespace ViewModels.CRUD.Interfaces
{
    public interface IMaterialMultiSelect
    {
        List<long> MaterialIds { get; set; }
        List<InventoryBriefViewModel> Materials { get; set; }
    }
}
