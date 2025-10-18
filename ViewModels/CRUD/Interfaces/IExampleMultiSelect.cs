namespace ViewModels.CRUD.Interfaces
{
    public interface IExampleMultiSelect
    {
        List<long> MultiSelectPropertyIds { get; set; }
        List<BaseSelect2VM> MultiSelectPropertyList { get; set; }
    }
}
