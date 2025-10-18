using DataLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IdentityStore
{
    public class ApplicationUserStore<TUser, TRole> : UserStore<TUser, TRole, ApplicationDbContext, long>
        where TUser : ApplicationUser
        where TRole : ApplicationRole
    {
        private readonly ApplicationDbContext db;
        public ApplicationUserStore(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }
        public async override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
        {

            bool combinationExists = await db.Users
            .AnyAsync(x => x.UserName == user.UserName
                        && x.Email == user.Email);
            if (combinationExists)
            {
                var IdentityError = new IdentityError { Description = "The specified username and email are already registered" };
                return IdentityResult.Failed(IdentityError);
            }
            return await base.CreateAsync(user);
        }
        public override Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default)
        {
            return base.AddLoginAsync(user, login, cancellationToken);
        }
        public override async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            return await Users.Where(u => u.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync();
        }
        public override async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            return await Users.Where(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync();
        }
    }
}
