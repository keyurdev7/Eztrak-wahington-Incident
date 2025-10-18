using Pagination;

namespace ViewModels
{
    public class MUTCDSearchViewModel : BaseSearchModel
    {
        public string? Code { get; set; }
        public override string OrderByColumn { get; set; } = "Code";

    }
}
