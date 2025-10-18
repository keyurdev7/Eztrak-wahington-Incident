using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

namespace IdentityManager
{
    public class ApplicationSignInManager<TUser>
        : SignInManager<TUser> where TUser : ApplicationUser
    {
        public ApplicationSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<TUser> confirmation
            )
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override Task SignInAsync(TUser user, bool isPersistent, string? authenticationMethod = null)
        {
            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = await UserManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    return SignInResult.Failed;
                }
            }
            return await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
        public override Task<SignInResult> PasswordSignInAsync(TUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            //if (user.PhoneNumberConfirmed == false)
            //{
            //    return Task.FromResult(SignInResult.TwoFactorRequired);
            //}
            return base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }
}
