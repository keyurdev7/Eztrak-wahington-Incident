using ViewModels.Shared;

namespace ViewModels
{
    public class EquipmentFieldsDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string? EquipmentFieldsID { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public long? HourlyRate { get; set; }
    }
}
