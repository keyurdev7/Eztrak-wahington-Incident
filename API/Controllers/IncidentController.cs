using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentDashboardService _dashboardService;

        public IncidentController(IIncidentDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get all incidents for map and recent list (same data as web dashboard map).
        /// Requires JWT (e.g. after PIN login). Use for mobile map view.
        /// </summary>
        [HttpGet]
        [Route("map")]
        public async Task<IActionResult> GetAllIncidentsForMap()
        {
            var report = await _dashboardService.GetIncidentDashboardReport();
            var incidentDashboard = report.IncidentDashboard;

            return Ok(new
            {
                incidentsOnMap = incidentDashboard.ListIncidentLocationMapViewModel,
                recentIncidents = incidentDashboard.ListIncidentRecentViewModel,
                statusCounts = new
                {
                    submitted = incidentDashboard.TotalSubmittedCount,
                    validated = incidentDashboard.TotalValidatedCount,
                    completed = incidentDashboard.TotalCompletedCount,
                    cancelled = incidentDashboard.TotalCancelledCount
                }
            });
        }
    }
}
