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
    public class WorkOrderStatusController : ApiBaseController
    {
        private readonly IRepositoryResponse _response;

        public WorkOrderStatusController(
            IRepositoryResponse response)
        {
            _response = response;
        }

        [HttpGet]
        [Route("/api/[controller]/Get")]
        public async Task<ActionResult> Get()
        {
            var replacementCycle = Enum.GetValues(typeof(WOStatusCatalog))
                   .Cast<WOStatusCatalog>().Where(x => x != WOStatusCatalog.Approved && x != WOStatusCatalog.OnHold)
                   .Select(e => new BaseMinimalVM { Name = e.GetDisplayName(), Id = (long)e })
                   .ToList();
            return Ok(replacementCycle);
        }
    }
}