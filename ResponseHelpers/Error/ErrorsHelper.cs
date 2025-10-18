using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Centangle.Common.ResponseHelpers.Error
{
    public static class ErrorsHelper
    {
        public static ModelStateDictionary AddErrorsToModelState(IdentityResult identityResult, ModelStateDictionary modelState, string code = null)
        {
            foreach (var e in identityResult.Errors)
            {
                var key = string.IsNullOrEmpty(code) ? e.Code : code;
                modelState.TryAddModelError(key, e.Description);
            }

            return modelState;
        }
        public static ModelStateDictionary AddErrorsToModelState(string code, List<string> errors, ModelStateDictionary modelState)
        {
            foreach (var e in errors)
            {
                modelState.TryAddModelError(code, e);
            }

            return modelState;
        }
        public static ModelStateDictionary AddErrorToModelState(string code, string description, ModelStateDictionary modelState)
        {
            modelState.TryAddModelError(code, description);
            return modelState;
        }

        public static void AddCommonErrorToModelState(ModelStateDictionary modelState)
        {
            AddErrorToModelState("", ErrorMessages.CommonError(), modelState);
        }
    }
}
