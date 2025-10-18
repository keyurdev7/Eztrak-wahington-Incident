using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Repositories.VersionService
{
    public class VersionService : IVersionService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VersionService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetVersionNumber()
        {
            var versionHeader = _configuration.GetValue<string>("VersionHeader");
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(versionHeader))
            {
                return _httpContextAccessor.HttpContext.Request.Headers[versionHeader];
            }
            return _configuration.GetValue<string>("DefaultVersion");
        }

        public bool GetIsUpdateForcible()
        {
            var isUpdateForcible = _configuration.GetValue<bool>("IsUpdateForcible");
            return isUpdateForcible;
        }

        public string GetLatestApiVersion()
        {
            var version = _configuration.GetValue<string>("LatestVersion");
            if (string.IsNullOrEmpty(version))
                return "";
            return version;
        }
    }
}
