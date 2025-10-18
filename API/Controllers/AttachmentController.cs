using System.Net;
using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Services.AttachmentService.Interface;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ApiBaseController
    {
        private readonly IAttachment _service;
        private readonly IMapper _mapper;
        private readonly ILogger<AttachmentController> _logger;
        private readonly IRepositoryResponse _response;

        public AttachmentController(IAttachment service,
            IMapper mapper
            , ILogger<AttachmentController> logger
            , IRepositoryResponse response)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
            _response = response;
        }

        [HttpDelete]
        [Route("/api/[controller]/Delete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _service.Delete(new List<long> { id });
            if (response.Status != HttpStatusCode.OK)
            {
                _logger.LogError("Asset Delete threw an error modelstate is not valid");
            }
            return ReturnProcessedResponse(response);
        }
    }
}