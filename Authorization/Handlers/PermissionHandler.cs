using Microsoft.AspNetCore.Authorization;
using DataLibrary;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Shared.UserInfoServices.Interface;
using Repositories.Common.Example.Interface;
using ViewModels.AssignedPermissions;
using Helpers.Authorization;

namespace BoilerPlate.Authorization.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly IUserInfoService _userInfo;
        private readonly IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> _assignedPermission;

        public PermissionHandler(
            ApplicationDbContext db,
            IMemoryCache cache,
            IUserInfoService userInfo,
            IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> assignedPermission)
        {
            _db = db;
            _cache = cache;
            _userInfo = userInfo;
            _assignedPermission = assignedPermission;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            string cacheKey = AuthorizationContants.CacheKey;
            var userId = _userInfo?.LoggedInUserId();
            long.TryParse(userId, out long parsedUserId);
            var roleIdList = _userInfo?.LoggedInUserRoleIds() ?? new List<long>();
            var roleIds = string.Join(",", roleIdList);
            var userIdentifierKey = AuthorizationHelper.GetIdentifierKey(userId, roleIds);

            var permissions = GetPermissionsFromCache(cacheKey, userIdentifierKey) ?? await RefreshUserPermissions(cacheKey, userIdentifierKey, parsedUserId, roleIdList);

            if (HasRequiredPermission(permissions, requirement.PermissionName))
            {
                context.Succeed(requirement);
            }
        }

        private List<string> GetPermissionsFromCache(string cacheKey, string userIdentifierKey)
        {
            if (_cache.TryGetValue(cacheKey, out Dictionary<string, string> permissions) && permissions?.Count > 0)
            {
                var permissionNames = "";
                permissions?.TryGetValue(userIdentifierKey, out permissionNames);
                if (!string.IsNullOrEmpty(permissionNames))
                {
                    return permissionNames.Split(",").ToList();
                }
            }
            return null;
        }

        private async Task<List<string>> RefreshUserPermissions(string cacheKey, string userIdentifierKey, long userId, List<long> roleIdList)
        {
            var permissions = await _assignedPermission.SetUserPermissions(cacheKey, userIdentifierKey, userId, roleIdList);
            return permissions;
        }

        private bool HasRequiredPermission(List<string> permissions, string requiredPermission)
        {
            if (permissions != null && permissions.Count > 0 && permissions.Any(x => x.ToLower().Trim() == requiredPermission.ToLower().Trim()))
                return true;
            return false;
        }

    }
}
