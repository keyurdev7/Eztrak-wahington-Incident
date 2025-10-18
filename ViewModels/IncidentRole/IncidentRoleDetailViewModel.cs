// ViewModels/IncidentRoleDetailViewModel.cs
using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class IncidentRoleDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
