using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels.DataTable;
using ViewModels;
using Web.Controllers.Shared;
using ViewModels;

namespace Web.Controllers
{
    public class AssetTypeController : CrudBaseController<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel, AssetTypeDetailViewModel, AssetTypeBriefViewModel, AssetTypeSearchViewModel>
    {
        public AssetTypeController(IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel> service, ILogger<AssetTypeController> logger, IMapper mapper) : base(service, logger, mapper, "AssetType", "Asset Type")
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name"},
                 new DataTableViewModel{title = "Color",data = "Color"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }
    }
}

