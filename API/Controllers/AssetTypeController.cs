using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;
using Repositories.Common;
using ViewModels;
using ViewModels.Shared;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTypeController : ApiBaseController
    {
        private readonly IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetTypeController> _logger;
        private readonly IRepositoryResponse _response;

        public AssetTypeController(IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel> service,
            IMapper mapper
            , ILogger<AssetTypeController> logger
            , IRepositoryResponse response)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _response = response;
        }
        [HttpGet]
        [Route("/api/[controller]/Get")]
        public async Task<ActionResult> Get()
        {
            var AssetTypeResponse = await _service.GetAll<AssetTypeDetailViewModel>(new AssetTypeSearchViewModel { DisablePagination = true });
            var resposne = AssetTypeResponse as IRepositoryResponseWithModel<PaginatedResultModel<AssetTypeDetailViewModel>>;
            var mapData = _mapper.Map<List<BaseMinimalVM>>(resposne.ReturnModel.Items);
            return Ok(mapData);
        }
    }
}