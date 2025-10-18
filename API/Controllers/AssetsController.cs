using System.Net;
using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Pagination;
using Repositories.Common;
using ViewModels;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ApiBaseController
    {
        private readonly IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetsController> _logger;
        private readonly IRepositoryResponse _response;

        public AssetsController(IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> service,
            IMapper mapper
            , ILogger<AssetsController> logger
            , IRepositoryResponse response)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _response = response;
        }
        [HttpGet]
        [Route("/api/[controller]/GetMapData")]
        public async Task<ActionResult> GetMapData()
        {
            var assetsResponse = await _service.GetAll<AssetDetailViewModel>(new AssetSearchViewModel() { ForMapData = true, DisablePagination = true });
            var resposne = assetsResponse as IRepositoryResponseWithModel<PaginatedResultModel<AssetDetailViewModel>>;
            var mapData = _mapper.Map<List<AssetMapViewModel>>(resposne.ReturnModel.Items);
            foreach (var item in mapData)
            {
                foreach (var association in item.AssetAssociations)
                {
                    item.AssetsTypeSubLevels.Add(association.AssetTypeLevel1.Name, association.AssetTypeLevel2.Name);
                }
            }
            return Ok(mapData);
        }

        [HttpGet]
        [Route("/api/[controller]/GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _service.GetById(id);
            if (response.Status == HttpStatusCode.OK)
            {
                try
                {
                    var responseModel = response as RepositoryResponseWithModel<AssetDetailViewModel>;
                    var asset = _mapper.Map<AssetDetailAPIViewModel>(responseModel?.ReturnModel);
                    var assetResponseModel = new RepositoryResponseWithModel<AssetDetailAPIViewModel>();

                    var assetTypeLevelResponse = (await _service.GetAssetTypeLevelsForAPI(asset.AssetType.Id, asset.Id)) as RepositoryResponseWithModel<List<AssetAssociationDetailAPIViewModel>>;
                    if (assetTypeLevelResponse?.Status == HttpStatusCode.OK)
                    {
                        asset.AssetAssociations = assetTypeLevelResponse.ReturnModel;
                    }
                    assetResponseModel.ReturnModel = asset;
                    return ReturnProcessedResponse<AssetDetailAPIViewModel>(assetResponseModel);
                }
                catch (Exception ex)
                {

                }
            }
            _logger.LogError("Asset GetById threw an error modelstate is not valid");
            return ReturnProcessedResponse(response);
        }

        [HttpPost]
        [Route("/api/[controller]/Post")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Post([FromForm] AssetCreateAPIViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createModel = _mapper.Map<AssetModifyViewModel>(model);
                var response = await _service.Create(createModel);
                if (response.Status == HttpStatusCode.OK)
                {
                    return ReturnProcessedResponse(response);
                }
            }
            _logger.LogError("Asset Post threw an error modelstate is not valid");
            return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        }

        [HttpPut]
        [Route("/api/[controller]/Put")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Put([FromForm] AssetModifyAPIViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateModel = _mapper.Map<AssetModifyViewModel>(model);
                var response = await _service.Update(updateModel);
                if (response.Status == HttpStatusCode.OK)
                {
                    return ReturnProcessedResponse(response);
                }
            }
            _logger.LogError("Asset Put threw an error modelstate is not valid");
            return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        }

        [HttpDelete]
        [Route("/api/[controller]/Delete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(long id)
        {

            var response = await _service.Delete(id);
            if (response.Status != HttpStatusCode.OK)
            {
                _logger.LogError("Asset Delete threw an error modelstate is not valid");
            }
            return ReturnProcessedResponse(response);
        }

        [HttpPost]
        [Route("/api/[controller]/SaveNotes")]
        public async Task<IActionResult> SaveNotes([FromForm] AssetNotesAPIViewModel model)
        {

            if (ModelState.IsValid)
            {
                var mappedModel = _mapper.Map<AssetNotesViewModel>(model);
                var response = await _service.SaveNotes(mappedModel);
                return ReturnProcessedResponse(response);
            }
            _logger.LogError("Asset SaveNotes threw an error modelstate is not valid");
            return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        }

        [HttpPut]
        [Route("/api/[controller]/RePin")]
        public async Task<IActionResult> RePin(AssetRePinViewModel model)
        {

            if (ModelState.IsValid)
            {
                var response = await _service.RePin(model);
                return ReturnProcessedResponse(response);
            }
            _logger.LogError("Asset SaveNotes threw an error modelstate is not valid");
            return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        }

        [HttpGet]
        [Route("/api/[controller]/GetNotesHistory")]
        public async Task<IActionResult> GetNotesHistory(int id)
        {
            var response = await _service.GetNotesByAssetId(id);
            if (response.Status == HttpStatusCode.OK)
            {
                var responseModel = response as RepositoryResponseWithModel<List<AssetNotesViewModel>>;
                return ReturnProcessedResponse<List<AssetNotesViewModel>>(responseModel);
            }
            _logger.LogError("Asset GetNotesHistory threw an error modelstate is not valid");
            return ReturnProcessedResponse(response);
        }

        [HttpPut]
        [Route("/api/[controller]/UploadImages")]
        public async Task<IActionResult> UploadImages([FromForm] AssetAddImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _service.UploadImages(model);
                return ReturnProcessedResponse(response);
            }
            return ReturnProcessedResponse(new RepositoryResponse
            {
                Status = HttpStatusCode.BadRequest
            });
        }

        [HttpPut]
        [Route("/api/[controller]/AssetAssociation")]
        public async Task<IActionResult> AssetAssociation(AssetAssociationModifyAPIViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var assetId = model.AssetAssociations.Select(x => x.AssetId).FirstOrDefault();
                    var assetTypeId = model.AssetAssociations.Select(x => x.AssetTypeId).FirstOrDefault();
                    var existingAssoications = (await _service.GetAssetTypeLevelsForAPI(assetTypeId, assetId)) as RepositoryResponseWithModel<List<AssetAssociationDetailAPIViewModel>>;
                    foreach (var ea in existingAssoications?.ReturnModel)
                    {
                        var newAssociation = model.AssetAssociations.Where(x => x.AssetTypeLevel1.Id == ea.AssetTypeLevel1.Id).FirstOrDefault();
                        if (newAssociation != null)
                        {
                            ea.AssetTypeLevel1.SelectedAssetTypeLevel2Id = newAssociation.AssetTypeLevel1.SelectedAssetTypeLevel2Id;
                        }
                    }
                    var mappedAssociations = _mapper.Map<List<AssetAssociationDetailViewModel>>(existingAssoications.ReturnModel);
                    var data = new AssetModifyViewModel
                    {
                        AssetAssociations = mappedAssociations,
                        Condition = new ConditionBriefViewModel
                        {
                            Id = model.ConditionId
                        }
                    };
                    var response = await _service.UpdateAssociations(data);
                    return ReturnProcessedResponse(response);
                }
                catch (Exception ex)
                {

                }

            }
            return ReturnProcessedResponse(new RepositoryResponse
            {
                Status = HttpStatusCode.BadRequest
            });
        }
    }
}