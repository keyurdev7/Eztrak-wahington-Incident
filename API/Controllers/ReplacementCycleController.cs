using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Shared;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ReplacementCycleController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ReplacementCycleController> _logger;
        private readonly IRepositoryResponse _response;

        public ReplacementCycleController(
            IMapper mapper
            , ILogger<ReplacementCycleController> logger
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
            var replacementCycle = Enum.GetValues(typeof(ReplacementCycleCatalog))
                   .Cast<ReplacementCycleCatalog>()
                   .Select(e => new BaseMinimalVM { Name = e.GetDisplayName(), Id = (long)e })
                   .ToList();
            return Ok(replacementCycle);
        }
    }
}