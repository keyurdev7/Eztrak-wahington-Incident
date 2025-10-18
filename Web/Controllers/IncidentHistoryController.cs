using Microsoft.AspNetCore.Mvc;
using Repositories.Services.IncidentHistory.Interface;

namespace Web.Controllers
{
    public class IncidentHistoryController : Controller
    {
        private readonly IIncidentHistoryService _historyService;

        public IncidentHistoryController(IIncidentHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet("IncidentHistory/GetHistoryModal")]
        public async Task<IActionResult> GetHistoryModal(long incidentId)
        {
            var vm = await _historyService.GetIncidentHistoryAsync(incidentId);
            return PartialView("_IncidentHistoryModal", vm);
        }
    }
}
