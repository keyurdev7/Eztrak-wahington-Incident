using Microsoft.AspNetCore.Authorization;

namespace Authorization.Attributes
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission) : base(policy: permission)
        {

        }

    }
}
