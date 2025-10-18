using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.Shared;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AssetTypeLevelController : ApiBaseController
    {
        private readonly IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetTypeLevelController> _logger;
        private readonly IRepositoryResponse _response;

        public AssetTypeLevelController(IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> service,
            IMapper mapper
            , ILogger<AssetTypeLevelController> logger
            , IRepositoryResponse response)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _response = response;
        }
        [HttpGet]
        [Route("/api/[controller]/Get")]
        public async Task<ActionResult> Get(long assetTypeId, long assetId)
        {
            var assetTypeLevelsResponse = await _service.GetAssetTypeLevels(assetTypeId, assetId);
            var assetTypeLevels = (assetTypeLevelsResponse as RepositoryResponseWithModel<List<AssetTypeLevel1DetailViewModel>>).ReturnModel;
            try
            {
                var mapData = _mapper.Map<List<AssetTypeLevel1DetailAPIViewModel>>(assetTypeLevels);
                return Ok(mapData);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}