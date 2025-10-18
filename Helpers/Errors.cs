using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Helpers.Models
{
    public static class Errors
    {
        public static ModelStateDictionary AddErrorsToModelState(IdentityResult identityResult, ModelStateDictionary modelState)
        {
            foreach (var e in identityResult.Errors)
            {
                modelState.TryAddModelError(e.Code, e.Description);
            }
            return modelState;
        }
        public static ModelStateDictionary AddErrorsToModelState(List<string> errors, ModelStateDictionary modelState)
        {
            foreach (var e in errors)
            {
                modelState.TryAddModelError("", e);
            }
            return modelState;
        }
        public static ModelStateDictionary AddErrorToModelState(string code, string description, ModelStateDictionary modelState)
        {
            modelState.TryAddModelError(code, description);
            return modelState;
        }
        //public static ModelStateDictionary AddErrorToModelState(Exception ex, ModelStateDictionary modelState)
        //{
        //    var exceptionMessage = ex.GetaAllMessages();
        //    modelState.AddModelError("Exception", exceptionMessage);
        //    return modelState;
        //}

        public static string GetaAllMessages(this Exception exception)
        {
            var messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message);
            return String.Join(Environment.NewLine, messages);
        }
        public static IEnumerable<TSource> FromHierarchy<TSource>(
                                                                this TSource source,
                                                                Func<TSource, TSource> nextItem,
                                                                Func<TSource, bool> canContinue
                                                                )
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }

        private static List<string> GetExceptionMessages(this Exception e)
        {
            var msgs = new List<string>();
            if (e == null) return msgs;
            if (msgs.Count()==0) msgs.Add(e.Message);
            if (e.InnerException != null)
                msgs.AddRange(GetExceptionMessages(e.InnerException));
            return msgs;
        }
        public static ModelStateDictionary AddGenericErrorIfRequired(string description, ModelStateDictionary modelState)
        {
            if (modelState.ErrorCount == 0)
                modelState.TryAddModelError("", description);
            return modelState;
        }
    }
}
