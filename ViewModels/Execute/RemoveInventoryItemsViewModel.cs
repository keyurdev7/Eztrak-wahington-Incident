using Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class RemoveInventoryItemsViewModel
    {
        public List<RemoveInventoryItemsListViewModel> Transactions { get; set; } = new();

    }
    public class RemoveInventoryItemsListViewModel : TransactionIssueViewModel
    {
        public RemoveInventoryJustificationCatalog Justification { get; set; }
        public double RemovedQuantity { get; set; }

        public IFormFile File { get; set; }

    }
}
