using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class AssetsController : Controller
    {
        public IActionResult Index(string activeTab)
        {
            var queryParams = Request.QueryString.ToString();
            var tab = new TabViewModel()
            {
                Id = "asset-tab",
                ActiveTab = activeTab,
                // CustomPartialContentPath = @"~/Views/WorkOrder/_InnerNavTab.cshtml",
                ContentId = "asset-tab-content-Id",
                Title = "Assets",
                TabItems = new List<TabItemViewModel>()
                {
                    new TabItemViewModel()
                    {
                        Id="home",
                        Name="Home",
                        Url="/Asset/Index"


                    },
                    new TabItemViewModel()
                    {
                        Id="asset-map",
                        Name="Map",
                        Url="/Map/Index?hideLayout=true&isForWebDashboard=true",
                    }
                }
            };
            return View(tab);
        }
    }
}
