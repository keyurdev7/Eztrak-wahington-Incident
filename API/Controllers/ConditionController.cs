using System.Net;
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
    public class ConditionController : ApiBaseController
    {
        private readonly IConditionService<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ConditionController> _logger;
        private readonly IRepositoryResponse _response;

        public ConditionController(IConditionService<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel> service,
            IMapper mapper
            , ILogger<ConditionController> logger
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
            var ConditionResponse = await _service.GetAll<ConditionDetailViewModel>(new ConditionSearchViewModel { DisablePagination = true });
            var resposne = ConditionResponse as IRepositoryResponseWithModel<PaginatedResultModel<ConditionDetailViewModel>>;
            var mapData = _mapper.Map<List<BaseMinimalVM>>(resposne.ReturnModel.Items);
            return Ok(mapData);
        }
    }
}