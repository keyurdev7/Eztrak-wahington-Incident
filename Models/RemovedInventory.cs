using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class RemovedInventory : BaseDBModel
    {
        public RemoveInventoryJustificationCatalog Justification { get; set; }
        public string? Url { get; set; }

    }
}
