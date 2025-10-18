using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using Models;

namespace ViewModels
{
    public class DynamicColumnModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Title", Prompt = "Title")]
        public string Title { get; set; }
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        public string? Regex { get; set; }
        public DynamicColumnType Type { get; set; }

        [Display(Name = "Table", Prompt = "Table")]
        public DynamicColumnEntityType EntityType { get; set; }

        public List<DynamicColumnOptionDetailViewModel>? Options { get; set; }
        public List<DynamicColumnValueDetailViewModel>? Values { get; set; }

    }
}
