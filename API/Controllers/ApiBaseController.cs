using Centangle.Common.ResponseHelpers.Error;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ViewModels.Shared;

namespace API.Controllers
{
    [Authorize]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult ReturnProcessedResponse(IRepositoryResponse response)
        {
            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    {
                        return Ok(new { Status = 200 });
                    }
                case HttpStatusCode.BadRequest:
                    {
                        if (ModelState.IsValid)
                            ErrorsHelper.AddErrorToModelState("", ErrorMessages.CommonError(), ModelState);

                        return BadRequest(new ValidationProblemDetails(ModelState));
                    }
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.Unauthorized:
                    return Unauthorized();
                default:
                    {
                        ErrorsHelper.AddErrorToModelState("", ErrorMessages.ProcessingError(), ModelState);
                        return BadRequest();
                    }
            }
        }

        protected IActionResult ReturnProcessedResponse<T>(IRepositoryResponse response) where T : new()
        {
            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    {
                        var result = new ResponseModel<T> { Status = 200 };
                        if (response is IRepositoryResponseWithModel<T>)
                            result.Data = ((IRepositoryResponseWithModel<T>)response).ReturnModel;
                        return Ok(result);
                    }
                case HttpStatusCode.BadRequest:
                    {
                        if (ModelState.IsValid)
                            ErrorsHelper.AddErrorToModelState("", ErrorMessages.CommonError(), ModelState);

                        var badResponse = new
                        {
                            errors = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                        };
                        return BadRequest(new ValidationProblemDetails(ModelState));
                    }
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.Unauthorized:
                    return Unauthorized();
                default:
                    {
                        ErrorsHelper.AddErrorToModelState("", ErrorMessages.ProcessingError(), ModelState);
                        return BadRequest();
                    }
            }
        }
    }
}
