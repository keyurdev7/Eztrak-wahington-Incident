using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Helpers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViewModels.Authentication;
using ViewModels.Employee;
using ViewModels.Users;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ApiBaseController
    {
        public IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;
        private readonly IRepositoryResponse _response;
        private readonly IMapper _mapper;

        public AccountController
            (
                IConfiguration configuration,
                ApplicationDbContext db,
                ILogger<AccountController> logger,
                IRepositoryResponse response,
                IMapper mapper
            )
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
            _response = response;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("/api/Account/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string pincode)
        {
            IRepositoryResponse result;
            try
            {
                var encodedPassCode = pincode.EncodePasswordToBase64();
                if (ModelState.IsValid)
                {
                    var user = await _db.Users.Where(x => x.PinCode == encodedPassCode).FirstOrDefaultAsync();
                    if (user?.ActiveStatus == Enums.ActiveStatus.Inactive)
                    {
                        ModelState.AddModelError("message", "Your account is in-active.");
                        result = Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response);
                        var check = ReturnProcessedResponse(result);
                        return check;
                    }

                    if (user != null)
                    {
                        var name = User?.Identity?.Name;
                        var fullName = $"{user?.FirstName} {user?.LastName}";

                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.FirstName),
                            new Claim("FullName", fullName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                        var token = new JwtSecurityToken
                            (
                                issuer: _configuration["JWT:ValidIssuer"],
                                audience: _configuration["JWT:ValidAudience"],
                                expires: DateTime.Now.AddHours(12),
                                claims: authClaims,
                                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                           );

                        var userDetail = _mapper.Map<UserBriefViewModel>(user);

                        var responseModel = new RepositoryResponseWithModel<TokenVM>();
                        responseModel.ReturnModel = new TokenVM
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            Expiry = token.ValidTo,
                            UserDetail = userDetail
                        };
                        return ReturnProcessedResponse<TokenVM>(responseModel);
                    }

                    else
                    {
                        ModelState.AddModelError("message", "Invalid login attempt.");
                        result = Centangle.Common.ResponseHelpers.Response.BadRequestResponse(_response);
                        return ReturnProcessedResponse(result);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _logger.LogError(ex.Message, "Login Endpoint Exception msg");
            }
            result = Centangle.Common.ResponseHelpers.Response.UnAuthorizedResponse(_response);
            return ReturnProcessedResponse(result);
        }
    }
}
