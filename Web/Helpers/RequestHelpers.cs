namespace Web.Helpers
{
    internal static class RequestHelpers
    {
        internal static bool IsAjaxRequest(this HttpRequest request)
        {
            return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
                string.Equals(request.Headers["X-Requested-With"], "Fetch", StringComparison.Ordinal);
        }
    }
}
