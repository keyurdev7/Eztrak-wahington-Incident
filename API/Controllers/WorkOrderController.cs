using System.Net;
using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;
using Repositories.Common;
using ViewModels;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrderController : ApiBaseController
    {
        private readonly IWorkOrderService<WorkOrderModifyViewModel, WorkOrderModifyViewModel, WorkOrderDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkOrderController> _logger;
        private readonly IRepositoryResponse _response;

        public WorkOrderController(IWorkOrderService<WorkOrderModifyViewModel, WorkOrderModifyViewModel, WorkOrderDetailViewModel> service,
            IMapper mapper
            , ILogger<WorkOrderController> logger
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
            var assetsResponse = await _service.GetAll<WorkOrderDetailViewModel>(new WorkOrderSearchViewModel() { DisablePagination = true });
            var resposne = assetsResponse as IRepositoryResponseWithModel<PaginatedResultModel<WorkOrderDetailViewModel>>;
            var mapData = _mapper.Map<List<WorkOrderBriefAPIViewModel>>(resposne.ReturnModel.Items);
            return Ok(mapData);
        }

        [HttpGet]
        [Route("/api/[controller]/GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            var response = await _service.GetById(id);
            if (response.Status == HttpStatusCode.OK)
            {
                var responseModel = response as RepositoryResponseWithModel<WorkOrderDetailViewModel>;
                return ReturnProcessedResponse<WorkOrderDetailViewModel>(responseModel);
            }
            _logger.LogError("WorkOrder GetById threw an error modelstate is not valid");
            return ReturnProcessedResponse(response);
        }

        [HttpPut]
        [Route("/api/[controller]/UploadImages")]
        public async Task<IActionResult> UploadImages([FromForm] WorkOrderAddImageViewModel model)
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

        [HttpPost]
        [Route("/api/[controller]/SaveNotes")]
        public async Task<IActionResult> SaveNotes([FromForm] WorkOrderNotesAPIViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mappedModel = _mapper.Map<WorkOrderNotesViewModel>(model);
                var response = await _service.SaveNotes(mappedModel);
                return ReturnProcessedResponse(response);
            }
            return ReturnProcessedResponse(new RepositoryResponse { Status = HttpStatusCode.BadRequest });
        }

        [HttpPut]
        [Route("/api/[controller]/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromForm] WorkOrderModifyStatusAPIViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mappedModel = _mapper.Map<WorkOrderModifyStatusViewModel>(model);
                var response = await _service.UpdateStatus(mappedModel);
                return ReturnProcessedResponse(response);
            }
            return ReturnProcessedResponse(new RepositoryResponse { Status = HttpStatusCode.BadRequest });
        }

        [HttpGet]
        [Route("/api/[controller]/GetNotesHistory")]
        public async Task<IActionResult> GetNotesHistory(int id)
        {
            var response = await _service.GetNotesByWorkOrderId(id);
            if (response.Status == HttpStatusCode.OK)
            {
                var responseModel = response as RepositoryResponseWithModel<List<WorkOrderNotesViewModel>>;
                return ReturnProcessedResponse<List<WorkOrderNotesViewModel>>(responseModel);
            }
            _logger.LogError("WorkOrder GetNotesHistory threw an error modelstate is not valid");
            return ReturnProcessedResponse(response);
        }


        //[HttpPost]
        //[Route("/api/[controller]/Post")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> Post([FromForm] WorkOrderCreateAPIViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var createModel = _mapper.Map<WorkOrderModifyViewModel>(model);
        //        var response = await _service.Create(createModel);
        //        if (response.Status == HttpStatusCode.OK)
        //        {
        //            return ReturnProcessedResponse(response);
        //        }
        //    }
        //    _logger.LogError("WorkOrder Post threw an error modelstate is not valid");
        //    return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        //}

        //[HttpPut]
        //[Route("/api/[controller]/Put")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> Put([FromForm] WorkOrderModifyAPIViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var updateModel = _mapper.Map<WorkOrderModifyViewModel>(model);
        //        var response = await _service.Update(updateModel);
        //        if (response.Status == HttpStatusCode.OK)
        //        {
        //            return ReturnProcessedResponse(response);
        //        }
        //    }
        //    _logger.LogError("WorkOrder Put threw an error modelstate is not valid");
        //    return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        //}

        //[HttpDelete]
        //[Route("/api/[controller]/Delete")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType((int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> Delete(long id)
        //{

        //    var response = await _service.Delete(id);
        //    if (response.Status == HttpStatusCode.OK)
        //    {
        //        return ReturnProcessedResponse(response);
        //    }
        //    _logger.LogError("WorkOrder Delete threw an error modelstate is not valid");
        //    return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
        //}
    }
}