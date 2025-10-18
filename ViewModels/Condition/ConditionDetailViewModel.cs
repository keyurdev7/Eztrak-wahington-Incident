using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class ConditionDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
