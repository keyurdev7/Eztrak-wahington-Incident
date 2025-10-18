using ViewModels.Shared;
using System;

namespace ViewModels
{
    public class MaterialDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string MaterialID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public long UnitCost { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
