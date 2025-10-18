using Models;
using Pagination;

namespace ViewModels
{
    public class EquipmentTransactionSearchViewModel : BaseSearchModel
    {
        public Supplier Supplier { get; set; } = new();
        public Location Location { get; set; } = new();
        public UOM UOM { get; set; } = new();
        public Equipment Equipment { get; set; } = new();
    }
}
