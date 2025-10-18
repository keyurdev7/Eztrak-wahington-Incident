using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class AssetTypeLevel1Controller : CrudBaseController<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1ModifyViewModel, AssetTypeLevel1DetailViewModel, AssetTypeLevel1DetailViewModel, AssetTypeLevel1BriefViewModel, AssetTypeLevel1SearchViewModel>
    {
        public AssetTypeLevel1Controller(IAssetTypeLevel1Service<AssetTypeLevel1ModifyViewModel, AssetTypeLevel1ModifyViewModel, AssetTypeLevel1DetailViewModel> service, ILogger<AssetTypeLevel1Controller> logger, IMapper mapper) : base(service, logger, mapper, "AssetTypeLevel1", "AssetTypeLevel1")
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "AssetType",data = "AssetType.Name"},
                new DataTableViewModel{title = "Name",data = "Name"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
    }
}

