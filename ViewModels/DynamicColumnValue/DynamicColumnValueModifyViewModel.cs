using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;

namespace ViewModels
{
    public class DynamicColumnValueModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {

        public string? Value { get; set; }
        public long EntityId { get; set; }

        public DynamicColumnDetailViewModel DynamicColumn { get; set; } = new();
    }
}
