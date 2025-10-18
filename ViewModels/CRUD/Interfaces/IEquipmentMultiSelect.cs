namespace ViewModels.CRUD.Interfaces
{
    public interface IEquipmentMultiSelect
    {
        List<long> EquipmentIds { get; set; }
        List<EquipmentBriefViewModel> Equipments { get; set; }
    }
}
