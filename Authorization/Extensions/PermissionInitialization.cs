using DataLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace BoilerPlate.Authorization
{
    /// <summary>
    /// Helper class for initializing permissions by retrieving them from the database.
    /// If we don't want to manually add policies during startup, we can use PermissionAuthorizationPolicyProvider.
    /// This provider checks if a policy is added; if not, it adds it at runtime.
    /// The PermissionAuthorizationPolicyProvider class is already implemented and should be added to ConfigureServices like:
    /// services.AddSingleton&lt;IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider&gt;();
    /// </summary>
    public static class PermissionInitialization
    {
        public static void AddPermission(this IServiceCollection services)
        {
            try
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var permissions = dbContext.Permissions.Select(p => p.Name).ToList();

                foreach (var permission in permissions)
                {
                    AddAuthorizationForPolicy(services, permission);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void AddAuthorizationForPolicy(IServiceCollection services, string permission)
        {
            services.AddAuthorizationCore(options =>
                options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)))
            );
        }

    }
}
