using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.VersionService;
using ViewModels.Authentication;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateStatusController : ApiBaseController
    {
        private readonly IVersionService _versionService;

        public UpdateStatusController(IVersionService versionService)
        {
            this._versionService = versionService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateStatus()
        {
            var responseModel = new RepositoryResponseWithModel<UpdateStatusVM>();
            responseModel.ReturnModel = new UpdateStatusVM { LatestVersion = _versionService.GetLatestApiVersion(), IsForcible = _versionService.GetIsUpdateForcible() };
            return ReturnProcessedResponse<UpdateStatusVM>(responseModel);
        }
    }
}

