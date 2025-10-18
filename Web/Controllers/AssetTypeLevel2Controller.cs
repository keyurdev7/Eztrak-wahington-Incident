using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class AssetTypeLevel2Controller : CrudBaseController<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2ModifyViewModel, AssetTypeLevel2DetailViewModel, AssetTypeLevel2DetailViewModel, AssetTypeLevel2BriefViewModel, AssetTypeLevel2SearchViewModel>
    {
        public AssetTypeLevel2Controller(IAssetTypeLevel2Service<AssetTypeLevel2ModifyViewModel, AssetTypeLevel2ModifyViewModel, AssetTypeLevel2DetailViewModel> service, ILogger<AssetTypeLevel2Controller> logger, IMapper mapper) : base(service, logger, mapper, "AssetTypeLevel2", "AssetTypeLevel2")
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                 new DataTableViewModel{title = "AssetType Level 1",data = "AssetTypeLevel1Name"},
                new DataTableViewModel{title = "Name",data = "Name"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
    }
}

