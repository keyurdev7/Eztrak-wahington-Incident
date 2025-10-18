using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class AssetIdController : CrudBaseController<
        AssetIdModifyViewModel,
        AssetIdModifyViewModel,
        AssetIdDetailViewModel,
        AssetIdDetailViewModel,
        AssetIdBriefViewModel,
        AssetIdSearchViewModel>
    {
        public AssetIdController(
            IAssetIdService<AssetIdModifyViewModel, AssetIdModifyViewModel, AssetIdDetailViewModel> service,
            ILogger<AssetIdController> logger,
            IMapper mapper)
            : base(service, logger, mapper, "AssetId", "AssetId", false)
        {
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Name",data = "Name", orderable = true},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
        }
    }
}
