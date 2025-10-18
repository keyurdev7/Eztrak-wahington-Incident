using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class WorkOrdersController : Controller
    {
        public IActionResult Index(string activeTab)
        {
            var queryParams = Request.QueryString.ToString();
            var tab = new TabViewModel()
            {
                Id = "work-order-tab",
                ActiveTab = activeTab,
                CustomPartialContentPath = @"~/Views/WorkOrder/_InnerNavTab.cshtml",
                ContentId = "work-order-tab-content-Id",
                Title = "Work Orders",
                TabItems = new List<TabItemViewModel>()
                {
                    new TabItemViewModel()
                    {
                        Id="dashboard",
                        Name="Home",
                        Url="/WorkOrder/Dashboard"


                    },
                    //new TabItemViewModel()
                    //{
                    //    Id="order-grid",
                    //    Name="Work Orders",
                    //    Url="/WorkOrder/Index",
                    //    Params=(activeTab=="order-grid"?queryParams:"")
                    //},
                    new TabItemViewModel()
                    {
                        Id="timesheet-approval",
                        Name="Timesheet Approvals",
                        Url="/Approval/ApprovalIndex",
                        Params=(activeTab=="timesheet-approval"?queryParams:""),
                        HideTopSearchBar=true,
                    },
                    new TabItemViewModel()
                    {
                        Id="service-request",
                        Name="Public Service Request",
                        Url="/StreetServiceRequest/Index"
                    }
                }
            };
            return View(tab);
        }
    }
}
