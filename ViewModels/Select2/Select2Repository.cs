using System;
using System.Collections.Generic;
using System.Linq;
using Pagination;
using Select2.Model;

namespace Select2
{
    public class Select2Repository
    {
        List<Select2ViewModel> GetPagedListOptions(int pageSize, int pageNumber, List<Select2ViewModel> list, out int totalSearchRecords)
        {
            //var allSearchedResults = GetAllSearchResults(searchTerm);
            totalSearchRecords = list.Count;
            return list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public Select2PagedResultViewModel GetSelect2PagedResult(int pageSize, int pageNumber, List<Select2ViewModel> list)
        {
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(pageSize, pageNumber, list, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        List<Select2OptionModel<T>> GetPagedListOptions<T>(int pageSize, int pageNumber, List<Select2OptionModel<T>> list, out int totalSearchRecords)
        {
            //var allSearchedResults = GetAllSearchResults(searchTerm);
            totalSearchRecords = list.Count;
            return list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public Select2PagedResult<T> GetSelect2PagedResult<T,M>(int pageSize, int pageNumber, List<Select2OptionModel<T>> list, PaginatedResultModel<M> items)
        {
            var select2pagedResult = new Select2PagedResult<T>();
            //var totalResults = 0;
            select2pagedResult.Results = list;// GetPagedListOptions(pageSize, pageNumber, list, out totalResults);
            select2pagedResult.Total = items._meta.TotalCount; 
            return select2pagedResult;
        }
    }
}
