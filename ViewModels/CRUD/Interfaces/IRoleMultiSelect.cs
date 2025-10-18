namespace ViewModels.CRUD.Interfaces
{
    public interface IRoleMultiSelect
    {
        List<long> RoleIds { get; set; }
        List<BaseSelect2VM> RoleList { get; set; }
    }
}
