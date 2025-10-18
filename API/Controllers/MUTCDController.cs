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
    public class MUTCDController : ApiBaseController
    {
        private readonly IMUTCDService<MUTCDModifyViewModel, MUTCDModifyViewModel, MUTCDDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly ILogger<MUTCDController> _logger;
        private readonly IRepositoryResponse _response;

        public MUTCDController(IMUTCDService<MUTCDModifyViewModel, MUTCDModifyViewModel, MUTCDDetailViewModel> service,
            IMapper mapper
            , ILogger<MUTCDController> logger
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
            var MUTCDResponse = await _service.GetAll<MUTCDDetailViewModel>(new MUTCDSearchViewModel { DisablePagination = true });
            var resposne = MUTCDResponse as IRepositoryResponseWithModel<PaginatedResultModel<MUTCDDetailViewModel>>;
            var mapData = _mapper.Map<List<BaseMinimalVM>>(resposne.ReturnModel.Items);
            return Ok(mapData);
        }
    }
}