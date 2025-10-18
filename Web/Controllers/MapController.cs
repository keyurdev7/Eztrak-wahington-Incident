using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using DocumentFormat.OpenXml;
using Helpers.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Pagination;
using Repositories.Common;
using System.Net;
using System.Security.Claims;
using ViewModels;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class MapController : Controller
    {
        private readonly IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> _service;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public MapController(
            IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> service,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db,
            IMapper mapper)
        {
            _service = service;
            _signInManager = signInManager;
            _db = db;
            _mapper = mapper;
        }
        public IActionResult Index(bool hideLayout = false,bool isForWebDashboard=false)
        {
            ViewBag.HideLayout = hideLayout;
            ViewBag.IsForWebDashboard = isForWebDashboard;
            return View();
        }

        public IActionResult Unauthorized()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetMapData()
        {
            var assetsResponse = await _service.GetAll<AssetDetailViewModel>(new AssetSearchViewModel() { ForMapData = true, DisablePagination = true });
            var resposne = assetsResponse as IRepositoryResponseWithModel<PaginatedResultModel<AssetDetailViewModel>>;
            var mapData = _mapper.Map<List<AssetMapViewModel>>(resposne.ReturnModel.Items);
            foreach (var item in mapData)
            {
                foreach (var association in item.AssetAssociations)
                {
                    item.AssetsTypeSubLevels.Add(association.AssetTypeLevel1.Name, association.AssetTypeLevel2.Name);
                }
            }
            return Ok(mapData);
        }
        public async Task<AssetMapViewModel> Get(long id)
        {
            try
            {
                var response = await _service.GetById(id);
                if (response.Status == HttpStatusCode.OK)
                {
                    var responseModel = response as RepositoryResponseWithModel<AssetDetailViewModel>;
                    var mapData = _mapper.Map<AssetMapViewModel>(responseModel.ReturnModel);

                    foreach (var association in mapData.AssetAssociations)
                    {
                        mapData.AssetsTypeSubLevels.Add(association.AssetTypeLevel1.Name, association.AssetTypeLevel2.Name);
                    }
                    return mapData;
                }
            }
            catch (Exception ex)
            {

            }

            return new AssetMapViewModel();
        }

        public async Task<IActionResult> Login()
        {
            try
            {
                var request = HttpContext.Request;
                Microsoft.Extensions.Primitives.StringValues customHeaderValue;
                request.Headers.TryGetValue("X-Pin-Header", out customHeaderValue);
                var pinCode = customHeaderValue.ToString();
                if (!string.IsNullOrEmpty(pinCode))
                {
                    var encodedPin = pinCode.EncodePasswordToBase64();
                    var user = await _db.Users.Where(x => x.PinCode == encodedPin).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, true);
                        var customClaims = new[] {
                                new Claim(ClaimTypes.Email, user.Email) };
                        await _signInManager.SignInWithClaimsAsync(user, true, customClaims);
                        return LocalRedirect("/Map/Index?hideLayout=true");
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return LocalRedirect("/Map/Unauthorized"); ;

        }
    }
}
