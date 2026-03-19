using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class EventTypeDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}