using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using ViewModels.Shared;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceCycleController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<MaintenanceCycleController> _logger;
        private readonly IRepositoryResponse _response;

        public MaintenanceCycleController(
            IMapper mapper
            , ILogger<MaintenanceCycleController> logger
            , IRepositoryResponse response)
        {
            _mapper = mapper;
            _logger = logger;
            _response = response;
        }
        [HttpGet]
        [Route("/api/[controller]/Get")]
        public async Task<ActionResult> Get()
        {
            var MaintenanceCycle = Enum.GetValues(typeof(MaintenanceCycleCatalog))
                   .Cast<MaintenanceCycleCatalog>()
                   .Select(e => new BaseMinimalVM { Name = e.GetDisplayName(), Id = (long)e })
                   .ToList();
            return Ok(MaintenanceCycle);
        }
    }
}