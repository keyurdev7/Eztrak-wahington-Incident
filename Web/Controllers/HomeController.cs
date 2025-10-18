using DataLibrary;

using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

using Humanizer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;

using Repositories.Common;
using Repositories.Services.Dashboard.Interface;
using Repositories.Services.Report.Interface;

using System.Diagnostics;
using System.Drawing;
using System.IO.Pipelines;

using ViewModels.Dashboard;
using ViewModels.Dashboard.Common.Card;
using ViewModels.Dashboard.interfaces;
using ViewModels.Incident;
using ViewModels.Report.PendingOrder;

using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _service;
        private readonly IIncidentDashboardService _iIncidentDashboardService;
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IDashboardService _dashboardService;
        private readonly IDashboardCardService _cardService;
        private readonly IDashboardTableService _tableService;
        private readonly IReportService _reportService;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IDashboardService service, IDashboardFactory dashboardFactory,
            IDashboardService dashboardService, IDashboardCardService cardService, IDashboardTableService tableService
            , IReportService reportService, ApplicationDbContext context, IIncidentDashboardService iIncidentDashboardService
            )
        {
            _logger = logger;
            _service = service;
            _dashboardFactory = dashboardFactory;
            _dashboardService = dashboardService;
            _cardService = cardService;
            _tableService = tableService;
            _reportService = reportService;
            _context = context;
            _iIncidentDashboardService = iIncidentDashboardService;
        }

        //public async Task<JsonResult> GetChartData(DashboardSearchViewModel search)
        //{
        //    var chartData = await _service.GetChartData(search);
        //    return Json(chartData);
        //}

        public IActionResult Index()
        {

            return RedirectToAction("Dashboard", "Home");
        }



        [HttpGet]
        public async Task<JsonResult> GetPendingOrderChartData(PendingOrderChartSearchViewModel search)
        {
            var chartData = await _dashboardService.GetPendingOrderChartData(search);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetPendingOrderCardData()
        {
            var chartData = await _cardService.GetPendingOrderCardData();
            return View("~/Views/Dashboard/Common/Card/_Content.cshtml", chartData);
        }

        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            var dashboardViewModel = await _iIncidentDashboardService.GetIncidentDashboardReport();
            return View("ChartIndex", dashboardViewModel);
        }
        [HttpGet]
        public async Task<ActionResult> GetWorkOrderChartData()
        {
            var chartData = await _reportService.GetWorkOrderChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetWorkOrderByAssetTypeChartData()
        {
            var chartData = await _reportService.GetWorkOrderByAssetTypeChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetWorkOrderByRepairTypeChartData()
        {
            var chartData = await _reportService.GetWorkOrderByRepairTypeChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetWorkOrderByTechnicianChartData()
        {
            var chartData = await _reportService.GetWorkOrderByTechnicianChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetWorkOrderByManagerChartData()
        {
            var chartData = await _reportService.GetWorkOrderByManagerChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetAverageOrderCompletionTimeCardData()
        {
            var response = new DashboardCardDataViewModel("fas fa-calendar-minus");
            response.Cards.Add(await _reportService.GetAverageOrderCompletionTime(null));
            return View("~/Views/Dashboard/Common/Card/_Content.cshtml", response);
        }

        [HttpGet]
        public async Task<ActionResult> GetAssetsByConditionChartData()
        {
            var chartData = await _reportService.GetAssetsByConditionChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetAssetsMaintenanceDueChartData()
        {
            var chartData = await _reportService.GetAssetsMaintenanceDueChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetAssetsReplacementDueChartData()
        {
            var chartData = await _reportService.GetAssetsReplacementDueChartData(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetAverageLaborCostCardData()
        {
            var cardData = new DashboardCardDataViewModel("fas fa-calendar-minus");
            cardData.Cards.Add(await _reportService.GetAverageLaborCost(null));
            return View("~/Views/Dashboard/Common/Card/_Content.cshtml", cardData);

        }

        [HttpGet]
        public async Task<ActionResult> GetAverageEquipmentCostCardData()
        {
            var cardData = new DashboardCardDataViewModel("fas fa-calendar-minus");
            cardData.Cards.Add(await _reportService.GetAverageEquipmentCost(null));
            return View("~/Views/Dashboard/Common/Card/_Content.cshtml", cardData);

        }

        [HttpGet]
        public async Task<ActionResult> GetAverageMaterialCostCardData()
        {
            var cardData = new DashboardCardDataViewModel("fas fa-calendar-minus");
            cardData.Cards.Add(await _reportService.GetAverageMaterialCost(null));
            return View("~/Views/Dashboard/Common/Card/_Content.cshtml", cardData);

        }

        [HttpGet]
        public async Task<ActionResult> GetCostAccuracyChartData()
        {
            var chartData = await _reportService.GetCostAccuracy(null);
            return Json(chartData);
        }

        [HttpGet]
        public async Task<ActionResult> GetPendingOrderTableData()
        {
            var data = await _tableService.GetPendingOrderTableData();
            return View("~/Views/Dashboard/PendingOrder/_Table.cshtml", data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<bool> ValidatePassword(string password)
        {
            var response = await _service.ValidatePassword(password);
            return response;
        }

        public async Task<IActionResult> SaveDataTableCell(string propertyName, string propertyValue, string entityId, string entityName)
        {
            var response = await _tableService.SaveDatatableCell(propertyName, propertyValue, entityId, entityName);
            return Json(response);
        }
    }
}