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
    public class ManufacturerController : ApiBaseController
    {
        private readonly IManufacturerService<ManufacturerModifyViewModel, ManufacturerModifyViewModel, ManufacturerDetailViewModel> _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ManufacturerController> _logger;
        private readonly IRepositoryResponse _response;

        public ManufacturerController(IManufacturerService<ManufacturerModifyViewModel, ManufacturerModifyViewModel, ManufacturerDetailViewModel> service,
            IMapper mapper
            , ILogger<ManufacturerController> logger
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
            var ManufacturerResponse = await _service.GetAll<ManufacturerDetailViewModel>(new ManufacturerSearchViewModel { DisablePagination = true });
            var resposne = ManufacturerResponse as IRepositoryResponseWithModel<PaginatedResultModel<ManufacturerDetailViewModel>>;
            var mapData = _mapper.Map<List<BaseMinimalVM>>(resposne.ReturnModel.Items);
            return Ok(mapData);
        }
    }
}