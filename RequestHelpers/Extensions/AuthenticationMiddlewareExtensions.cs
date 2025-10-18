using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Centangle.Common.RequestHelpers.Extensions
{
    public static class AuthenticationMiddlewareExtensions
    {
        public static void ConfigureAuthenticationHandler(this IApplicationBuilder appBuilder, ILoggerFactory loggerFactory)
        {
            appBuilder.Use(async (context, next) =>
            {
                var _logger = loggerFactory.CreateLogger("AuthenticationMiddlewareExtensions");
                try
                {
                    string authHeader = context.Request.Headers["Authorization"];
                    if (authHeader != null)
                    {
                        var accessToken = authHeader.Replace("Bearer ", "").Replace("bearer ", "");
                        var handler = new JwtSecurityTokenHandler();
                        context.User = handler.ValidateToken(accessToken, CreateTokenValidationParameters(), out var tokenSecure);
                    }
                    else
                    {
                        _logger.LogCritical("Auth header not set", "BiolerPlate.AuthenticationMiddlewareExtensions");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BiolerPlate.AuthenticationMiddlewareExtensions");
                }
                await next();
            });
        }
        public static TokenValidationParameters CreateTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)),
                //comment this and add this line to fool the validation logic
                SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                {
                    var jwt = new JwtSecurityToken(token);
                    return jwt;
                },
                RequireExpirationTime = false,
                ValidateLifetime = false,
                RequireSignedTokens = false,
                ClockSkew = TimeSpan.Zero,
            };
        }
    }
}
