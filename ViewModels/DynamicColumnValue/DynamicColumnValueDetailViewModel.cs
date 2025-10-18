using System.ComponentModel.DataAnnotations;
using Enums;
using Models;
using ViewModels.Shared;

namespace ViewModels
{
    public class DynamicColumnValueDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string? Value { get; set; }
        public long EntityId { get; set; }

        public DynamicColumnDetailViewModel DynamicColumn { get; set; } = new();
    }
}
