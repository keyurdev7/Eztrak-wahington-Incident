using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class StatusLegendDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }   // <- added
        public string Color { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
