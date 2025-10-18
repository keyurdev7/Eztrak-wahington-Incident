using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Security.Claims;

namespace IdentityStore.ClaimHelpers
{
    public static class ClaimsHelper
    {
        public static async Task<List<Claim>> GetClaims(ApplicationUser user, ApplicationDbContext db)
        {
            var roles = await (
                from ur in db.UserRoles
                join r in db.Roles on ur.RoleId equals r.Id
                where ur.UserId == user.Id
                select new { ur = ur, r = r }
                ).ToListAsync();
            var roleIds = roles.Select(x => x.ur.RoleId).Distinct().ToList();
            var email = !string.IsNullOrEmpty(user.Email) ? user.Email : "";
            var fullName = string.IsNullOrEmpty(user.NormalizedUserName) ? "" : user.NormalizedUserName;
            var claims = new List<Claim>
            {
                new Claim("RoleIds", String.Join(",", roleIds)),
                new Claim("Email", email.ToString()),
                new Claim("FullName", fullName.ToString())
            };
            return claims;
        }
    }
}
