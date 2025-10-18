using Helpers.Extensions;
using ViewModels.Shared;
using System;

namespace ViewModels
{
    public class IncidentShiftDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
