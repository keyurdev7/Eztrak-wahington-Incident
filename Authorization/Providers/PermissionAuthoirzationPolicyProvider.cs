using BoilerPlate.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Authorization.Providers
{
    /// <summary>
    /// Custom authorization policy provider that extends DefaultAuthorizationPolicyProvider.
    /// This provider checks if a policy is added; if not, it adds it at runtime.
    /// </summary>
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        /// <summary>
        /// Initializes a new instance of the PermissionAuthorizationPolicyProvider class.
        /// </summary>
        /// <param name="options">The options for configuring the authorization system.</param>
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> option)
            : base(option)
        {

        }
        /// <summary>
        /// Gets the authorization policy for the specified policy name.
        /// If the policy already exists, it is returned; otherwise, a new policy is created and returned.
        /// </summary>
        /// <param name="policyName">The name of the policy.</param>
        /// <returns>
        /// The authorization policy associated with the specified name.
        /// If the policy doesn't exist, a new policy with a PermissionRequirement is created.
        /// </returns>
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

            //var policyNameWithSpace = Regex.Replace(policyName, "([a-z])([A-Z])", "$1 $2");
            if (policy is not null)
            {
                return policy;
            }
            return new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

        }

    }
}
