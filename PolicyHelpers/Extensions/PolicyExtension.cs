//using BoilerPlate.Policy;
//using Microsoft.Extensions.DependencyInjection;

//namespace BoilerPlate.PolicyHelpers
//{
//    public static class PolicyInitialization
//    {
//        public static void AddPolicies(this IServiceCollection services)
//        {
//            try
//            {
//                Dictionary<string, string> policyRequirement = new Dictionary<string, string> {
//                { "CreateProject", "Create Project"},
//                { "TargetAndProjectCreationPolicy", "TargetAndProjectCreation" },
//                { "EditViewAndApprovePolicy", "EditViewAndApprove" },
//                { "AddUsersPolicy", "AddUsers" },
//                { "DefinitionCreationPolicy", "DefinitionCreation" }
//            };

//                foreach (var entry in policyRequirement)
//                {
//                    AddAuthorizationForPolicy(services, entry.Key, entry.Value);
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }

//        public static void AddAuthorizationForPolicy(IServiceCollection services, string permissionName, string requirement)
//        {
//            services.AddAuthorizationCore(options =>
//                options.AddPolicy(permissionName, policy => policy.Requirements.Add(new PolicyRequirement(requirement)))
//            );
//        }

//    }
//}
