//using Microsoft.AspNetCore.Authorization;
//using DataLibrary;
//using Microsoft.Extensions.Caching.Memory;
//using Repositories.Shared.UserInfoServices.Interface;
//using Repositories.Common.Example.Interface;
//using ViewModels.AssignedPermissions;

//namespace BoilerPlate.Policy.Handlers
//{
//    public class PermissionHandler : AuthorizationHandler<PolicyRequirement>
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IMemoryCache _cache;
//        private readonly IUserInfoService _userInfo;
//        private readonly IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> _assignedPermission;

//        public PermissionHandler(
//            ApplicationDbContext db,
//            IMemoryCache cache,
//            IUserInfoService userInfo,
//            IAssignedPermissionService<AssignedPermissionUpdateViewModel, AssignedPermissionUpdateViewModel, AssignedPermissionDetailViewModel> assignedPermission)
//        {
//            _db = db;
//            _cache = cache;
//            _userInfo = userInfo;
//            _assignedPermission = assignedPermission;
//        }

//        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
//        {
//            // Key for caching
//            var cacheKey = "permissions";

//            var requiredPolicyName = requirement.PermissionName.ToLower().Trim();
//            var permissionNameList = new List<string>();
//            var userExistsInCache = false;

//            var roleIdList = _userInfo.LoggedInUserRoleIds() ?? new();
//            var roleIds = string.Join(",", roleIdList);
//            var userId = _userInfo.LoggedInUserId();
//            long.TryParse(userId, out long parsedUserId);
//            var userIdentifierKey = $"#{userId}#,{roleIds},#";

//            // Try to get data from cache
//            if (_cache.TryGetValue(cacheKey, out Dictionary<string, string> permissions) && permissions?.Count > 0)
//            {
//                var permissionNames = "";
//                permissions?.TryGetValue(userIdentifierKey, out permissionNames);
//                userExistsInCache = !string.IsNullOrEmpty(permissionNames);
//                permissionNameList = permissionNames?.Split(",").ToList() ?? new();
//            }
//            if (!userExistsInCache)
//            {
//                permissionNameList = await _assignedPermission.SetUserPermissions(cacheKey, userIdentifierKey, parsedUserId, roleIdList);
//            }
//            if (permissionNameList != null && permissionNameList.Count > 0 && permissionNameList.Any(x => x.ToLower().Trim() == requiredPolicyName))
//                context.Succeed(requirement);
//        }

//    }
//}
