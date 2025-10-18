namespace ViewModels
{
    public class EquipmentShipmentModifyViewModel : EquipmentTransactionModifyViewModel
    {

    }

    public class EquipmentShipmentGridViewModel
    {
        public List<EquipmentShipmentModifyViewModel> EquipmentShipments { get; set; } = new();
    }
}

