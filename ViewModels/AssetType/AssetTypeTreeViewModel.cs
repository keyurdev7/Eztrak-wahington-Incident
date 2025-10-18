using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetTypeTreeViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public List<AssetTypeLevel1TreeViewModel> AssetTypeLevel1 { get; set; }

    }
}
