using DocumentFormat.OpenXml.Office2010.Excel;

using Microsoft.AspNetCore.Mvc;

using Models;

using Newtonsoft.Json;

using Repositories.Common;

using ViewModels;
using ViewModels.Incident;

namespace Web.Controllers
{
    public class ValidationController : Controller
    {
        private readonly IIncidentValidationService _iIncidentValidationService;
        public ValidationController(IIncidentValidationService iIncidentValidationService)
        {
            _iIncidentValidationService = iIncidentValidationService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long id)
        {
            TempData["TempComRecords"] = null;

            ValidationWorkflowViewModel validationWorkflow = new();
            validationWorkflow.Id = id;
            return View(validationWorkflow);
        }

        [HttpGet]
        public async Task<IActionResult> GetValidationsList()
        {
            ValidationWorkflowViewModel validationWorkflow = new();

            var pendingValidations = await _iIncidentValidationService.GetValidationPendingList();
            validationWorkflow.IVCount.PendingValidationCount = pendingValidations.Count;
            validationWorkflow.IVPendingList = pendingValidations;

            var recentlyAddedValidations = await _iIncidentValidationService.GetRecentlyValidationList();
            validationWorkflow.IVCount.TodayValidationCount = recentlyAddedValidations.Count;
            validationWorkflow.IVRecentlyList = recentlyAddedValidations;

            validationWorkflow.IVCount.HighSeverityCount = await _iIncidentValidationService.GetHighPriorityIncidentCount();

            return PartialView("_IncidentValidationDashboard", validationWorkflow);
        }

        [HttpGet]
        public async Task<IActionResult> GetValidationsDetail(long id)
        {
            ValidationWorkflowViewModel validationWorkflow = new();

            validationWorkflow.Id = id;

            var incidentValidationDtl = await _iIncidentValidationService.GetIncidentValidationDetail(id);
            validationWorkflow.IVDetails = incidentValidationDtl;

            var incidentValidationAlarm = await _iIncidentValidationService.GetIncidentValidationAlarm(id);
            validationWorkflow.IVValidation = incidentValidationAlarm;


            var incidentResponseTeams = await _iIncidentValidationService.GetIncidentValidationResponseTeam();
            validationWorkflow.IVResponseTeamList = incidentResponseTeams;

            var incidentPolicy = await _iIncidentValidationService.GetIncidentValidationPolicy();
            validationWorkflow.IVPolicyList = incidentPolicy;

            var incidentUsersTeam = await _iIncidentValidationService.GetIncidentTeamUsers();
            validationWorkflow.IVIncidentTeamUser = incidentUsersTeam;

            var additionalsLocs = await _iIncidentValidationService.GetIncidentAdditionalLocationByIncident(id);
            validationWorkflow.IVAdditionalLocations = additionalsLocs;

            return PartialView("_IncidentValidationDetail", validationWorkflow);
        }

        [HttpPost]
        public async Task<JsonResult> SavePolicy([FromBody] PolicyModifyViewModel request)
        {
            long policyId = await _iIncidentValidationService.SavePolicy(request);
            var teamsList = await _iIncidentValidationService.GetTeamsList();
            request.Id = policyId;

            return new JsonResult(new
            {
                Success = true,
                AssignTeams = teamsList,
                Request = request,
                Message = "Policy saved successfully"
            });
        }

        [HttpPost]
        public async Task<PartialViewResult> AddCommunicationRecord([FromForm] IncidentSubmitCommunicationViewModel request)
        {
            var newRecord = new IncidentSubmitCommunicationViewModel();

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "CommunicationTemp");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Save uploaded files
            if (request.Files != null && request.Files.Count > 0)
            {
                foreach (var file in request.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var filePath = Path.Combine(uploadsPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        newRecord.FileMeta.Add(new FileMeta
                        {
                            FileName = fileName,
                            OriginalName = file.FileName,
                            TempPath = filePath,
                        });
                    }
                }
            }

            newRecord.Message = request.Message;
            newRecord.MessageType = request.MessageType;
            newRecord.RecipientsIds = request.RecipientsIds;
            newRecord.RecipientNames = request.RecipientNames;
            newRecord.TimeStamp = request.TimeStamp;
            newRecord.UserName = request.UserName;

            // Load existing from TempData
            List<IncidentSubmitCommunicationViewModel> tempRecords;
            if (TempData["TempComRecords"] != null)
            {
                tempRecords = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<List<IncidentSubmitCommunicationViewModel>>(TempData["TempComRecords"].ToString());
            }
            else
            {
                tempRecords = new List<IncidentSubmitCommunicationViewModel>();
            }

            tempRecords.Add(newRecord);

            // Save back to TempData
            TempData["TempComRecords"] = Newtonsoft.Json.JsonConvert.SerializeObject(tempRecords);
            TempData.Keep("TempComRecords");

            // Return partial view with model
            return PartialView("_CommunicationHistory", tempRecords);
        }

        [HttpPost]
        public async Task<IActionResult> SaveIncidentValidation([FromForm] IncidentSubmitViewModel request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {

                var validationLocations = !string.IsNullOrWhiteSpace(request.listValidationLocationVM) ? JsonConvert
                    .DeserializeObject<List<IncidentValidationLocationViewModel>>(request.listValidationLocationVM) : new List<IncidentValidationLocationViewModel>();

                IncidentValidationLocationViewModel incidentValidationLocationViewModel = new IncidentValidationLocationViewModel();

                incidentValidationLocationViewModel.ICPLocation = request.IncidentLocation;
                incidentValidationLocationViewModel.Source = request.Source;
                incidentValidationLocationViewModel.DiscoveryPerimeter = request.DiscoveryPerimeterId;
                incidentValidationLocationViewModel.SeverityID = request.ConfirmedSeverityLevelId;
                //incidentValidationLocationViewModel.SeverityID = request.ValidationNotes;

                request.listSubmitValidationLocationVM.Add(incidentValidationLocationViewModel);
                var resultId = await _iIncidentValidationService.SaveIncidentValidation(request);

                


                var successMsg = $"Incident validation saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveIncidentValidation1([FromForm] IncidentSubmitViewModel request)
        {
            if (request == null)    
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {

                var policies = request.listPolicyVM != null ? JsonConvert
                    .DeserializeObject<List<IncidentSubmitPolicyViewModel>>(request.listPolicyVM) : new List<IncidentSubmitPolicyViewModel>();

                var validationLocations = !string.IsNullOrWhiteSpace(request.listValidationLocationVM) ? JsonConvert
                    .DeserializeObject<List<IncidentValidationLocationViewModel>>(request.listValidationLocationVM) : new List<IncidentValidationLocationViewModel>();

                var communications = JsonConvert
                    .DeserializeObject<List<IncidentSubmitCommunicationViewModel>>(TempData["TempComRecords"]?.ToString() ?? "");

                var personalInfo = !string.IsNullOrWhiteSpace(request.listPersonalDataVM) ? JsonConvert
                   .DeserializeObject<List<IncidentValidationPersonalViewModel>>(request.listPersonalDataVM) : new List<IncidentValidationPersonalViewModel>();

                var taskInfo = !string.IsNullOrWhiteSpace(request.listTaskDataVM) ? JsonConvert
                  .DeserializeObject<List<IncidentValidationTaskViewModel>>(request.listTaskDataVM) : new List<IncidentValidationTaskViewModel>();

                var CloseouttaskInfo = !string.IsNullOrWhiteSpace(request.listCloseoutTaskDataVM) ? JsonConvert
                  .DeserializeObject<List<IncidentValidationCloseoutTaskViewModel>>(request.listCloseoutTaskDataVM) : new List<IncidentValidationCloseoutTaskViewModel>();

                var incidentValidationAssessment = !string.IsNullOrWhiteSpace(request.incidentValidationAssessment) ? JsonConvert
                   .DeserializeObject<IncidentValidationAssessment>(request.incidentValidationAssessment) : new IncidentValidationAssessment();

                request.listSubmitPolicyVM = policies ?? new List<IncidentSubmitPolicyViewModel>();

                request.listSubmitCommunicationVM = communications ?? new List<IncidentSubmitCommunicationViewModel>();

                request.listSubmitValidationLocationVM = validationLocations ?? new List<IncidentValidationLocationViewModel>();

                request.listSubmitPersonalDataVM = personalInfo ?? new List<IncidentValidationPersonalViewModel>();

                request.listSubmitTaskDataVM = taskInfo ?? new List<IncidentValidationTaskViewModel>();
                request.listSubmitCloseoutTaskDataVM = CloseouttaskInfo ?? new List<IncidentValidationCloseoutTaskViewModel>();

                request.incidentSubmitValidationAssessment = incidentValidationAssessment ?? new IncidentValidationAssessment();

                var resultId = await _iIncidentValidationService.SaveIncidentValidation(request);

                var successMsg = $"Incident validation saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddLocation([FromBody] AddLocationRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Address))
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            try
            {
                // 1) Geocode
                var geo = await _iIncidentValidationService.GetLatLngFromAddress(request.Address);
                var lat = geo?.Lat ?? 0;
                var lng = geo?.Lng ?? 0;

                // 2) Save to AdditionalLocations
                var addedId = await _iIncidentValidationService.AddAdditionalLocationAsync(request.IncidentId, request.Address, lat, lng);

                if (addedId > 0)
                {
                    return Json(new
                    {
                        success = true,
                        item = new
                        {
                            id = addedId,
                            address = request.Address,
                            lat = lat,
                            lng = lng
                        }
                    });
                }

                return Json(new { success = false, message = "Failed to save location." });
            }
            catch (Exception ex)
            {
                // you already have _logger in controller? If not, use try/catch and return generic
                return Json(new { success = false, message = "Error saving location." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteAdditionalLocation([FromBody] AddLocationRequest request)
        {
            if (request == null)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            try
            {
                long id = await _iIncidentValidationService.DeleteAdditionalLocationAsync(request.IncidentId);
                if (id > 0)
                {
                    return Json(new { success = true, message = "Success" });
                }
                return Json(new { success = false, message = "Failed to delete location." });
            }
            catch (Exception ex)
            {
                // you already have _logger in controller? If not, use try/catch and return generic
                return Json(new { success = false, message = "Error delete location." });
            }
        }

        
    }
}
