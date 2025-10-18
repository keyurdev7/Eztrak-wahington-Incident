using Microsoft.AspNetCore.Mvc;
using Centangle.Common.ResponseHelpers.Error;
using Centangle.Common.ResponseHelpers.Models;
using System.Linq;
using System.Net;

namespace Centangle.Common.ResponseHelpers.ControllerHelpers
{
    public abstract class BaseController : ControllerBase
    {
        #region[Helper Function]
        protected IActionResult ReturnProcessedResponse<T>(IRepositoryResponse response) where T : new()
        {
            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    {
                        if (response is IRepositoryResponseWithModel<T> model)
                            return Ok(model.ReturnModel);
                        return Ok(response);
                    }
                case HttpStatusCode.BadRequest:
                    {
                        if (ModelState.IsValid)
                            ErrorsHelper.AddErrorToModelState("message", ErrorMessages.CommonError(), ModelState);

                        var badResponse = new
                        {
                            errors = ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage))
                        };
                        return BadRequest(new ValidationProblemDetails(ModelState));
                    }
                case HttpStatusCode.NotFound:
                    return NotFound(response);
                case HttpStatusCode.Unauthorized:
                    return Unauthorized(response);
                default:
                    {
                        ErrorsHelper.AddErrorToModelState("message", ErrorMessages.ProcessingError(), ModelState);
                        return BadRequest();
                    }
            }
        }
        protected IActionResult ReturnProcessedResponse(IRepositoryResponse response)
        {
            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    {
                        return Ok(response);
                    }
                case HttpStatusCode.BadRequest:
                    {
                        if (ModelState.IsValid)
                            ErrorsHelper.AddErrorToModelState("message", ErrorMessages.CommonError(), ModelState);
                        return BadRequest(new ValidationProblemDetails(ModelState));
                    }
                case HttpStatusCode.NotFound:
                    return NotFound(response);
                case HttpStatusCode.Unauthorized:
                    return Unauthorized(response);
                default:
                    {
                        ErrorsHelper.AddErrorToModelState("message", ErrorMessages.ProcessingError(), ModelState);
                        return BadRequest();
                    }
            }
        }
        #endregion
    }
}
