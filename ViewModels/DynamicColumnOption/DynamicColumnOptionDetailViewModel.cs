using Enums;
using ViewModels.Shared;

namespace ViewModels
{
    public class DynamicColumnOptionDetailViewModel : BaseCrudViewModel
    {

        public string Text { get; set; }
        public string Value { get; set; }
        public DynamicColumnType Type { get; set; }

        public DynamicColumnBriefViewModel DynamicColumn { get; set; }
    }
}
