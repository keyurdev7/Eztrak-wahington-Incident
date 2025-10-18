using Microsoft.AspNetCore.Mvc;

using Repositories.Common;

using System.Threading.Tasks;

using ViewModels;
using ViewModels.Incident;

namespace Web.Controllers
{

    public class IncidentValidationController : Controller
    {
        private readonly IIncidentService _iIncidentService;
        public IncidentValidationController(IIncidentService incidentService)
        {
            _iIncidentService = incidentService;
        }

        public async Task<ActionResult> Index()
        {
            var incidentViewModel = await _iIncidentService.GetIncidentDropDown();
            return View(incidentViewModel);
        }
    }
}