using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class RequestsController : Controller
    {
        public IActionResult Index(string activeTab)
        {
            var queryParams = Request.QueryString.ToString();
            var tab = new TabViewModel()
            {
                Id = "work-order-tab",
                ActiveTab = activeTab,
                CustomPartialContentPath = @"~/Views/Requests/_InnerNavTab.cshtml",
                ContentId = "work-order-tab-content-Id",
                Title = "Requests",
                TabItems = new List<TabItemViewModel>()
                {
                    new TabItemViewModel()
                    {
                        Id="dashboard",
                        Name="Request",
                        Url="/WorkOrder/Dashboard",
                        Prefix = "<i class='fa fa-home'></i>",
                    }
                }
            };
            return View(tab);
        }
    }
}

