using Pagination;

namespace ViewModels.CRUD
{
    public class DataCardViewModel<M>
    {
        public PaginatedResultModel<M> PaginatedResult { get; set; } = new PaginatedResultModel<M>();
        public int CurrentPage { get; set; }
        public int RecordsPerPage { get; set; }
        public bool DisablePagination { get; set; } = false;
        public int LastPage
        {
            get
            {
                return (int)Math.Ceiling((PaginatedResult._meta.TotalCount / (double)RecordsPerPage));
            }
        }
    }
}
