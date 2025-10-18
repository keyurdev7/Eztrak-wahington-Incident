using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;

namespace ViewModels
{
    public class DynamicColumnOptionModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {

        public string Text { get; set; }
        public string Value { get; set; }
        public DynamicColumnType Type { get; set; }

        public DynamicColumnBriefViewModel DynamicColumn { get; set; }
    }
}
