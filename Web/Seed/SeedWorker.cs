using DataLibrary;
using Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IdentityProvider.Seed
{
    public class SeedWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedWorker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //await context.Database.EnsureCreatedAsync(cancellationToken);
            // Check if the database exists before attempting to migrate

            try
            {
                // Attempt to create the database
                context.Database.EnsureCreated();

                // If EnsureCreated() does not throw an exception, it means the database exists
                // Proceed with your logic
            }
            catch (Exception ex)
            {
                // If an exception occurs, it could mean that the database does not exist
                // Handle the situation accordingly
            }
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync(cancellationToken);
            }

            try
            {
                bool roleAdded = false;
                foreach (var roleEnum in Enum.GetValues(typeof(RolesCatalog)).Cast<RolesCatalog>())
                {
                    var roleName = roleEnum.ToString();
                    var normalizedRoleName = roleName.ToUpper();

                    var existingRole = context.Roles.FirstOrDefault(r => r.NormalizedName == normalizedRoleName);

                    if (existingRole == null)
                    {
                        var newRole = new ApplicationRole()
                        {
                            Name = roleName,
                            NormalizedName = normalizedRoleName,
                            ConcurrencyStamp = Guid.NewGuid().ToString(), // Generate a unique concurrency stamp
                        };

                        context.Roles.Add(newRole);
                        roleAdded = true;
                    }
                }
                if (roleAdded)
                {
                    context.SaveChanges();
                }
                var alreadyCreatedUsers = context.Users.Any();
                if (!alreadyCreatedUsers)
                {
                    var user = new ApplicationUser();
                    user.FirstName = "Administrator";
                    user.ActiveStatus = ActiveStatus.Active;
                    await _userStore.SetUserNameAsync(user, "admin@eztrak.com", CancellationToken.None);
                    user.Email = "admin@eztrak.com";
                    if ((await _userManager.CreateAsync(user, "LAC@1234")).Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, RolesCatalog.SystemAdministrator.ToString());

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    }
}
