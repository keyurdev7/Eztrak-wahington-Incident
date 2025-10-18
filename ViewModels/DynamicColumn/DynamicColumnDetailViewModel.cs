using System.ComponentModel.DataAnnotations;
using Enums;
using Helpers.Extensions;
using ViewModels.Shared;

namespace ViewModels
{
    public class DynamicColumnDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        [Display(Name = "Title", Prompt = "Title")]
        public string Title { get; set; }
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        public string? Regex { get; set; }
        public DynamicColumnType Type { get; set; }
        public string FormattedType { get => Type.GetEnumDescription(); }

        [Display(Name = "Table", Prompt = "Table")]
        public DynamicColumnEntityType EntityType { get; set; }
        public string FormattedEntityType { get => EntityType.GetEnumDescription(); }

        public List<DynamicColumnOptionBriefViewModel>? Options { get; set; }
        public List<DynamicColumnValueDetailViewModel>? Values { get; set; }
    }
}
