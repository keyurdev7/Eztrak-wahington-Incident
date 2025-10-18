using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class ReplaceDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
