using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Authorization
{
    public static class AuthorizationHelper
    {
        public static string GetIdentifierKey(string? userId, string roleIds)
        {
            return $"{GetUserIdentifierKey(userId)}{GetRoleIdentifierKey(roleIds)}#";
        }
        public static string GetUserIdentifierKey(string? userId)
        {
            return $"#{userId}#";
        }
        public static string GetRoleIdentifierKey(string roleIds)
        {
            return $",{roleIds},";
        }

    }
}
