using DocumentFormat.OpenXml.Presentation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

using Repositories.Common;
using Repositories.Services.ArcGis;
using Repositories.Services.ArcGis.Interface;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.Incident;

namespace Web.Controllers
{

    public class IncidentsController : Controller
    {
        private readonly IIncidentService _iIncidentService;
        private readonly IArcGisGeocodingService _iArcGisGeocodingService;
        //private readonly IAdditionalLocationsService _iAdditionalLocationsService;

        public IncidentsController(IIncidentService incidentService, IArcGisGeocodingService iArcGisGeocodingService)
        {
            _iIncidentService = incidentService;
            _iArcGisGeocodingService = iArcGisGeocodingService;
        }

        public async Task<ActionResult> Index()
        {
            var incidentViewModel = await _iIncidentService.GetIncidentDropDown();
            return View(incidentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveIncident([FromForm] IncidentViewModel incidentViewModel)
        {
            if (incidentViewModel == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var incidentId = string.Empty;
                if (incidentViewModel.Id > 0)
                {
                    incidentId = await _iIncidentService.UpdateIncident(incidentViewModel);
                }
                else
                {
                    incidentId = await _iIncidentService.SaveIncident(incidentViewModel);                   
                }
                if (string.IsNullOrWhiteSpace(incidentId))
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save incident." });

                var successMsg = $"Incident {incidentId} saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<PartialViewResult> GetIncidentList([FromBody] FilterRequest request)
        {
            var incidentViewModel = new IncidentViewModel
            {
                incidentGridViewModel = await _iIncidentService.GetIncidentList(request)
            };

            return PartialView("_IncidentGrid", incidentViewModel ?? new IncidentViewModel());
        }


        //[HttpPost]
        //public async Task<IActionResult> ChangeIncidentStatus([FromBody] ChangeStatusRequest request)
        //{
        //    if (request == null || request.IncidentId <= 0 || string.IsNullOrWhiteSpace(request.Status))
        //    {
        //        return BadRequest(new { success = false, message = "Invalid data." });
        //    }

        //    var result = await _iIncidentService.ChangeIncidentStatus(request.IncidentId, request.Status);

        //    if (string.IsNullOrEmpty(result))
        //    {
        //        return NotFound(new { success = false, message = "Incident not found." });
        //    }

        //    return Ok(new { success = true, data = $"Incident {result} status changed successfully." });
        //}

        [HttpGet]
        public async Task<IActionResult> AddIncident()
        {
            var incidentViewModel = await _iIncidentService.GetIncidentDropDown();
            return PartialView("_AddEditIncidentModal", incidentViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditIncident(int id)
        {
            var model = await _iIncidentService.GetById(id);
            return PartialView("_AddEditIncidentModal", model);
        }
        [HttpGet]
        public async Task<PartialViewResult> GetIncidentDetails(long id)
        {
            var model = await _iIncidentService.GetIncidentDetailsById(id);
            return PartialView("_IncidentAllDetails", model);
        }

        [HttpGet]
        public async Task<IActionResult> Suggest(string text)
        {
            var results = await _iArcGisGeocodingService.GetSuggestionsAsyncWithMagicKey(text);
            return Json(results.Select(r => new { text = r.Text, magicKey = r.MagicKey }));
        }
       
        [HttpGet]
        public async Task<IActionResult> Resolve(string magicKey)
        {
            var result = await _iArcGisGeocodingService.GetCoordinatesAsync(magicKey);
            return Json(new
            {
                text = result.GetValueOrDefault().address,
                lat = result.GetValueOrDefault().lat,
                lon = result.GetValueOrDefault().lon
            });
        }
    
        [HttpGet]
        public async Task<PartialViewResult> GetIncidentMapDetailsbyId(long id)
        {
            var map = await _iIncidentService.GetIncidentMapDetailsbyId(id);
            return PartialView("_IncidentMap", map ?? new List<ViewModels.Dashboard.IncidentLocationMapViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> SaveCommunicationMessage([FromForm] SaveCommunicationRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { success = false, message = "Message is required." });

            try
            {

                var result = await _iIncidentService.SaveCommunicationMessage(request);
                if (result)
                    return Ok(new { success = true, message = "Message sent successfully." });
                else
                    return StatusCode(500, new { success = false, message = "Failed to save message." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving the message." });
            }
        }
        [HttpGet]
        public async Task<PartialViewResult> GetAdditionalLocations(long incidentId)
        {
            var locations = await _iIncidentService.GetAdditionalLocationsByIncidentId(incidentId);
            return PartialView("_AdditionalLocations", locations ?? new List<AdditionalLocationViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> GetLatLong(string text)
        {
            var results = await _iArcGisGeocodingService.GetSuggestionsAsynclat(text);
            return Json(results);
        }
    }
}