using ViewModels.Shared;

namespace ViewModels
{
    public class PolicyDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}