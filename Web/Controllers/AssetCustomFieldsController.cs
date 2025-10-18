using AutoMapper;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class AssetCustomFieldsController : DynamicColumnController
    {
        public AssetCustomFieldsController(IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnDetailViewModel> service, ILogger<DynamicColumnController> logger, IMapper mapper) : base(service, logger, mapper, "AssetCustomFields", "Asset Custom Fields", DynamicColumnEntityType.Asset)
        {
        }
    }
}

