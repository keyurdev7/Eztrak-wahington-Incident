using ViewModels.DataTable;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.DataTable
{
    public class DatatablePaginatedResultModel<M> : PaginatedResultModel<M>
    {
        public DatatablePaginatedResultModel(int draw, int totalCount, List<M> items)
        {
            this.draw = draw;
            recordsTotal = recordsFiltered = totalCount;
            Items = items;
            ActionsList = new List<DataTableActionViewModel>();
        }
        public int draw { get; set; }
        public int recordsTotal { get; set; }// Total Count Without any where clause only for  informational purpose
        public int recordsFiltered { get; set; }// Total Count after all filteration need to be applied. This needs to be set
        public bool ProcessManuallyForDT { get; set; }
        public bool ShowSelectedFilters { get; set; } = true;

        public Dictionary<string, string> AdditionalData = new Dictionary<string, string>();
        public List<DataTableActionViewModel> ActionsList { get; set; }


    }
}
