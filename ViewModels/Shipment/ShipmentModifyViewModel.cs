namespace ViewModels
{
    public class ShipmentModifyViewModel : TransactionModifyViewModel
    {

    }

    public class ShipmentGridViewModel
    {
        public List<ShipmentModifyViewModel> Shipments { get; set; } = new();
    }
}

