using DataLibrary;
using IdentityStore.ClaimHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models;
using System.Security.Claims;

namespace IdentityStore
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserClaimsPrincipalFactory(
                                                    UserManager<ApplicationUser> userManager,
                                                    RoleManager<ApplicationRole> roleManager,
                                                    IOptions<IdentityOptions> optionsAccessor,
                                                    ApplicationDbContext db
                                                    )
                                                    : base(userManager, roleManager, optionsAccessor)
        {
            _db = db;
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaims(await ClaimsHelper.GetClaims(user, _db));
            return identity;
        }

    }
}
