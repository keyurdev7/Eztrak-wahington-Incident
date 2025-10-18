using ViewModels.Shared;
using Pagination;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.DataTable
{

    public class BaseDateSearchModel : BaseSearchModel
    {
        public BaseDateSearchModel()
        {
            //FromDate = DateTime.Now.AddDays(-90);
            //ToDate = DateTime.Now.AddDays(1);
        }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ToDate { get; set; }
    }
}
