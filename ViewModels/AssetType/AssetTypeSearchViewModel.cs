using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AssetTypeSearchViewModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public string Color { get; set; }
        public override string OrderByColumn { get; set; } = "Name";

    }
}
