using Microsoft.AspNetCore.Http;
using Repositories.Shared.UserInfoServices.Interface;
using System.Security.Claims;

namespace Repositories.Shared.UserInfoServices
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string LoggedInUserId()
        {
            var userId = _httpContextAccessor?.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

        public string LoggedInEmployeeId()
        {
            var empId = _httpContextAccessor?.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return empId;
        }

        public long LoggedInSupplierId()
        {
            var supplierId = 0l;
            var supplierIdClaim = _httpContextAccessor?.HttpContext.User.FindFirstValue("SupplierId");
            try
            {
                supplierId = long.Parse(supplierIdClaim);
            }
            catch (Exception ex)
            {

            }
            return supplierId;
        }

        public string LoggedInUserImageUrl()
        {
            var imageUrl = _httpContextAccessor?.HttpContext.User.FindFirstValue("ImageUrl");
            return imageUrl;
        }

        public string LoggedInUserRole()
        {
            var role = _httpContextAccessor?.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            return role;
        }

        public string LoggedInWebUserRole()
        {
            var role = _httpContextAccessor?.HttpContext.User.FindFirstValue("Role");
            return role;
        }

        public string LoggedInUserDesignation()
        {
            var role = _httpContextAccessor?.HttpContext.User.FindFirstValue("Designation");
            return role;
        }

        public List<string> LoggedInUserRoles()
        {
            var roles = _httpContextAccessor?.HttpContext.User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();
            return roles;
        }

        public List<long> LoggedInUserRoleIds()
        {
            var roleIdString = _httpContextAccessor?.HttpContext.User.FindFirst(c => c.Type == "RoleIds")?.Value;
            var roleIds = roleIdString?.Split(',').Select(long.Parse).ToList();
            return roleIds ?? new();
        }

        public string LoggedInUserEmail()
        {
            var email = _httpContextAccessor?.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return email;
        }

        public string LoggedInUserFullName()
        {
            var name = _httpContextAccessor?.HttpContext.User.FindFirstValue("FullName");
            return name;
        }
    }
}
