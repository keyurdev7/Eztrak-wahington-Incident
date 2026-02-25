using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViewModels.Authentication;
using ViewModels.Users;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ApiBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;
        private readonly IRepositoryResponse _response;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IConfiguration configuration,
            ApplicationDbContext db,
            ILogger<AccountController> logger,
            IRepositoryResponse response,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
            _response = response;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>Login with 4-digit PIN (e.g. for Technician role). Returns JWT.</summary>
        [HttpPost]
        //[Route("/api/Account/Login")]
        [Route("/api/Account/LoginWithPin")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginWithPinRequest body)
        {
            var pin = body?.PinCode?.Trim();
            if (string.IsNullOrEmpty(pin) || pin.Length != 4 || !pin.All(char.IsDigit))
            {
                ModelState.AddModelError("message", "Pin code must be exactly 4 digits.");
                return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
            }

            IRepositoryResponse result;
            try
            {
                var encodedPassCode = pin.EncodePasswordToBase64();
                var user = await _db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.PinCode == encodedPassCode && !x.IsDeleted);

                if (user == null)
                {
                    ModelState.AddModelError("message", "Invalid login attempt.");
                    result = Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response);
                    return ReturnProcessedResponse(result);
                }

                if (user.ActiveStatus == Enums.ActiveStatus.Inactive)
                {
                    ModelState.AddModelError("message", "Your account is inactive.");
                    result = Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response);
                    return ReturnProcessedResponse(result);
                }

                await UpdateLastLoginAsync(user.Id);
                return BuildJwtResponse(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login with PIN failed.");
                result = Centangle.Common.ResponseHelpers.Response.UnAuthorizedResponse(_response);
                return ReturnProcessedResponse(result);
            }
        }

        /// <summary>Login with email and password. Returns JWT.</summary>
        [HttpPost]
        [Route("/api/Account/LoginWithPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithPassword([FromBody] LoginVM model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("message", "Email and password are required.");
                return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email.Trim());
                if (user == null || user.IsDeleted)
                {
                    ModelState.AddModelError("message", "Invalid login attempt.");
                    return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
                }

                if (user.ActiveStatus == Enums.ActiveStatus.Inactive)
                {
                    ModelState.AddModelError("message", "Your account is inactive.");
                    return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
                }

                var isValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isValid)
                {
                    ModelState.AddModelError("message", "Invalid login attempt.");
                    return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response));
                }

                await UpdateLastLoginAsync(user.Id);
                return BuildJwtResponse(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login with password failed.");
                return ReturnProcessedResponse(Centangle.Common.ResponseHelpers.Response.UnAuthorizedResponse(_response));
            }
        }

        private async Task UpdateLastLoginAsync(long userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        private IActionResult BuildJwtResponse(ApplicationUser user)
        {
            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrEmpty(fullName)) fullName = user.UserName ?? user.Email ?? "";

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName ?? user.UserName ?? ""),
                new Claim("FullName", fullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(12),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            var userDetail = _mapper.Map<UserBriefViewModel>(user);
            var responseModel = new RepositoryResponseWithModel<TokenVM>
            {
                ReturnModel = new TokenVM
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiry = token.ValidTo,
                    UserDetail = userDetail
                }
            };
            return ReturnProcessedResponse<TokenVM>(responseModel);
        }
    }

    /// <summary>Request body for PIN login (e.g. { "pinCode": "1234" }).</summary>
    public class LoginWithPinRequest
    {
        public string PinCode { get; set; }
    }
}
