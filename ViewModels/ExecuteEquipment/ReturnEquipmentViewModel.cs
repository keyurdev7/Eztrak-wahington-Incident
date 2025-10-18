namespace ViewModels
{
    public class ReturnEquipmentViewModel
    {
        public List<ReturnEquipmentListViewModel> Transactions { get; set; } = new();

    }
    public class ReturnEquipmentListViewModel : EquipmentTransactionIssueViewModel
    {
        public double ReturnedQuantity { get; set; }
        public double Hours { get; set; }

    }
}
