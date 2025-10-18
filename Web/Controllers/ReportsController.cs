using Microsoft.AspNetCore.Mvc;
using ViewModels.CRUD;

namespace Web.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index(string activeTab)
        {
            var tab = new TabViewModel()
            {
                Id = "report-tab",
                ActiveTab = activeTab,
                ContentId = "report-tab-content-Id",
                Title = "Report",
                TabItems = new List<TabItemViewModel>()
                {
                    new TabItemViewModel()
                    {
                        Id="maintenance-report",
                        Name="Maintenance",
                        Url="/Report/Maintenance"

                    },
                    new TabItemViewModel()
                    {
                        Id="replacement-report",
                        Name="Replacement",
                        Url="/Report/Replacement"

                    },
                    new TabItemViewModel()
                    {
                        Id="time-sheet-report",
                        Name="TimeSheet",
                        Url="/Report/TimeSheet"

                    },
                    new TabItemViewModel()
                    {
                        Id="transaction-report",
                        Name="Transaction",
                        Url="/Report/Transaction"

                    }
                }
            };
            return View(tab);
        }
    }
}
