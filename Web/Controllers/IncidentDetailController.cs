using DocumentFormat.OpenXml.Office2010.Excel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;

using Models;

using Newtonsoft.Json;

using Repositories.Common;

using ViewModels.Incident;

using Web.Extensions;

namespace Web.Controllers
{
    public class IncidentDetailController : Controller
    {
        private readonly IIncidentService _iIncidentService;
        private readonly IIncidentValidationService _iIIncidentValidationService;

        public object JsonRequestBehavior { get; private set; }

        public IncidentDetailController(IIncidentService incidentService, IIncidentValidationService iIIncidentValidationService)
        {
            _iIncidentService = incidentService;
            _iIIncidentValidationService = iIIncidentValidationService;
        }
        public async Task<IActionResult> Index(long id)
        {
            AssestmentFilterRequest request = new()
            {
                IncidentId = id
            };

            var model = await _iIncidentService.GetIncidentDetailsById(id);
            model.Id = id;
            var incidentValidationDtl = await _iIIncidentValidationService.GetIncidentValidationDetail(id);
            model.IVDetails = incidentValidationDtl;
            model.ListIncidentLocationMapViewModel = await _iIncidentService.GetIncidentMapDetailsbyId(id);
            model.listIncidentMapChats = await _iIncidentService.GetIncidentMapChatChat(id);
            model.IncidentAssessmentDetails = await _iIncidentService.GetAssessmentDetails(request);
            model.incidentViewAssessmentAttachmentView = await _iIncidentService.ViewAssessmentAttachment(id);
            model.incidentViewRestorationAttachmentView = await _iIncidentService.ViewRestorationAttachment(id);
            model.incidentViewCloseOutAttachmentView = await _iIncidentService.ViewClouseOutAttachment(id);
            model.TotalCompletedCloseOut = await _iIncidentService.GetTaskClouseOutCompletedCount(id);
            #region Personnel
            var companies = await _iIncidentService.GetAllCompanies();
            ViewBag.Companies = new SelectList(companies, "CompanyId", "CompanyName");
            var roles = await _iIncidentService.GetAllIncidentRoles();
            ViewBag.Roles = new SelectList(roles, "IncidentRoleId", "IncidentRoleName");

            var userdrop = await _iIncidentService.GetAllUsersDrop();
            ViewBag.UsersDrop = userdrop;
            var companiesdrop = await _iIncidentService.GetAllCompaniesDrop();
            ViewBag.CompaniesDrop = companiesdrop;
            var rolesdrop = await _iIncidentService.GetAllIncidentRolesDrop();
            ViewBag.RolesDrop = rolesdrop;
            var shifts = await _iIncidentService.GetAllShiftsDrop();
            ViewBag.ShiftsDrop = shifts;
            var statuses = await _iIncidentService.GetAllProgressStatus();
            ViewBag.Statuses = statuses.Select(s => new SelectListItem
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusName
            }).ToList();
            #endregion

            return View(model);
        }


        #region Map
        [HttpPost]
        public async Task<JsonResult> AddMapChat([FromBody] IncidentMapChatRequest request)
        {
            if (request == null)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            try
            {
                long id = await _iIncidentService.AddMapChat(request);
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
        #endregion

        #region Assestment
        [HttpPost]
        public async Task<PartialViewResult> GetAssessmentDetails([FromBody] AssestmentFilterRequest request)
        {
            var model = await _iIncidentService.GetAssessmentDetails(request);
            return PartialView("_IncidentAssessmentDetailsPartial", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> AddAssessmentDetails()
        {

            var model = await _iIncidentService.AddAssessmentDetails();
            return PartialView("_AddAssestmentPartial", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> EditAssessmentDetails(long id, long mainstepId, long substepId)
        {
            var model = await _iIncidentService.EditAssessmentDetails(id, mainstepId, substepId);
            return PartialView("_UpdateAssestmentPartial", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> ViewAssessmentDetails(long id, long mainstepId, long substepId)
        {
            var model = await _iIncidentService.ViewAssessmentDetails(id, mainstepId, substepId);
            return PartialView("_ViewAssestmentPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAssessment(IncidentAssessmentEditViewModel model, List<IFormFile> Files)
        {
            try
            {
                var fileUrls = new List<string>();

                if (Files != null && Files.Count > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "Assessment");

                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    foreach (var file in Files)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(uploadsPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Relative URL for use in browser
                            var relativeUrl = $"/Storage/uploads/Assessment/{fileName}";
                            fileUrls.Add(relativeUrl);
                        }
                    }
                    model.ImageUrl = string.Join(",", fileUrls);
                }

                long id = await _iIncidentService.UpdateAssessment(model);

                if (id > 0)
                {
                    AssestmentFilterRequest request = new()
                    {
                        IncidentId = model?.IncidentId ?? 0
                    };


                    var viewattachment = await _iIncidentService.ViewAssessmentAttachment(request.IncidentId);
                    var viewattachmentHtml = await this.RenderViewAsync("_ViewAttachmentAssestmentPartial", viewattachment, true);



                    var details = await _iIncidentService.GetAssessmentDetails(request);

                    return Json(new
                    {
                        success = true,
                        files = fileUrls,
                        asssetDetails = details,
                        partials = new
                        {
                            viewattachment = viewattachmentHtml,
                        }
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to Update Assessment." });
                }
            }
            catch (Exception ex)
            {
                // log ex here if needed
                return Json(new { success = false, message = "Save failed. " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> SubmitAssestment([FromForm] IncidentAssessmentSubmitRequest request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var assessment = !string.IsNullOrWhiteSpace(request.incidentValidationAssessment)
                    ? JsonConvert.DeserializeObject<IncidentValidationAssessment>(request.incidentValidationAssessment)
                    : new IncidentValidationAssessment();

                assessment.IncidentId = request.IncidentId;

                // TODO: Call your save service when ready
                var resultId = await _iIncidentService.SubmitAssestment(assessment);

                AssestmentFilterRequest requestFilter = new()
                {
                    IncidentId = request.IncidentId
                };

                var details = await _iIncidentService.GetAssessmentDetails(requestFilter);

                var successMsg = "Assestment saved successfully!";
                return Ok(new { success = true, data = successMsg, asssetDetails = details });
            }
            catch (Exception ex)
            {
                // Optionally log ex here
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An unexpected error occurred while saving the incident validation."
                });
            }
        }

        #endregion

        #region Personnel
        [HttpPost]
        public async Task<IActionResult> UpdateTimeIn(long id, DateTime timeIn)
        {
            try
            {
                var resultModel = await _iIncidentService.UpdateTimeIn(id, timeIn);

                if (resultModel == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to Update TimeIn." });

                return Ok(new { success = true, data = resultModel });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        [HttpGet]
        public IActionResult FilterByRole(long incidentId, long roleId, long companyid, string onsite)
        {
            try
            {
                var response = _iIncidentService.GetFilterByRole(incidentId, roleId, companyid, onsite);
                // Simply return Ok() with JSON data
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        [HttpGet]
        public IActionResult GetSupervisors(long companyId, long userId)
        {
            try
            {
                var response = _iIncidentService.GetSupervisors(companyId, userId);
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSupervisor(long personnelId, long supervisorId)
        {
            try
            {
                var Id = await _iIncidentService.UpdateSupervisor(personnelId, supervisorId);

                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to Update Supervisor." });

                var successMsg = "Update Supervisor successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddPerson(long userId, long companyId, long roleId, long shiftId, long incidentId, long incidentValidationId)
        {
            try
            {
                var resultModel = await _iIncidentService.AddPerson(userId, companyId, roleId, shiftId, incidentId, incidentValidationId);

                if (resultModel == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to Add Person." });

                return Ok(new { success = true, data = resultModel });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        #endregion

        #region ValidationNOte
        [HttpPost]
        public async Task<JsonResult> SaveValidationNote([FromBody] SaveValidationNoteRequest request)
        {
            if (request == null || request.IncidentId <= 0 || string.IsNullOrWhiteSpace(request.Notes))
                return Json(new { success = false, message = "Invalid request." });

            try
            {
                var id = await _iIncidentService.SaveValidationNoteAsync(request);
                if (id > 0)
                    return Json(new { success = true, id, message = "Saved" });

                return Json(new { success = false, message = "Save failed." });
            }
            catch (Exception ex)
            {
                // log if you have logger
                return Json(new { success = false, message = "Error saving note." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePostDetails([FromForm] IncidentViewPostViewModel incidentViewPostViewModel)
        {
            try
            {

                List<IncidentViewPostViewModel> listIncidentViewPostViewModel = await _iIncidentService.SavePostDetails(incidentViewPostViewModel);

                return Ok(new { success = true, data = listIncidentViewPostViewModel });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveTask([FromForm] IncidentViewTaskListViewModel model)
        {
            if (model == null || model.IncidentId == 0)
                return BadRequest(new { success = false, message = "Invalid request." });

            try
            {
                // call service to add task and return the saved entity/viewmodel
                var created = await _iIncidentService.AddIncidentTaskAsync(new AddIncidentTaskRequest
                {
                    IncidentId = model.IncidentId,
                    IncidentValidationId = model.IncidentValidationId,
                    TaskDescription = model.Task,
                    RoleIds = model.RoleIds,    // we'll pass RoleIds string in FieldValue or change model
                    StatusId = model.StatusId
                });

                // created should return a IncidentViewTaskListViewModel (or similar)
                return Ok(new { success = true, data = created });
            }
            catch (Exception ex)
            {
                // log...
                return StatusCode(500, new { success = false, message = "Save failed." });
            }
        }
        #endregion

        #region Restoration

        [HttpGet]
        public async Task<PartialViewResult> GetRestorationDetails(long id)
        {
            var model = await _iIncidentService.GetvalidationTaskVM(id);
            IncidentViewTaskViewModel validationTaskVM = new IncidentViewTaskViewModel
            {
                listIncidentViewTaskViewModel = model,
            };

            return PartialView("_RestorationChecklistPartial", validationTaskVM);
        }

        [HttpGet]
        public async Task<PartialViewResult> EditRestorationDetails(long id)
        {
            var model = await _iIncidentService.EditRestorationDetails(id);
            return PartialView("_UpdateRestorationPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRestoration(IncidentEditTaskListViewModel model, List<IFormFile> Files)
        {
            try
            {
                var fileUrls = new List<string>();

                if (Files != null && Files.Count > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "Restoration");

                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    foreach (var file in Files)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(uploadsPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Relative URL for use in browser
                            var relativeUrl = $"/Storage/uploads/Restoration/{fileName}";
                            fileUrls.Add(relativeUrl);
                        }
                    }
                    model.ImageUrl = string.Join(",", fileUrls);
                }

                long id = await _iIncidentService.UpdateRestoration(model);

                if (id > 0)
                {
                    AssestmentFilterRequest request = new()
                    {
                        IncidentId = model?.IncidentId ?? 0
                    };


                    var viewattachment = await _iIncidentService.ViewRestorationAttachment(request.IncidentId);
                    var viewattachmentHtml = await this.RenderViewAsync("_ViewAttachmentAssestmentPartial", viewattachment, true);

                    var resortaiondtl = await _iIncidentService.GetvalidationTaskVM(request.IncidentId);

                    IncidentViewTaskViewModel validationTaskVM = new IncidentViewTaskViewModel
                    {
                        listIncidentViewTaskViewModel = resortaiondtl,
                    };

                    var restorationHtml = await this.RenderViewAsync("_RestorationChecklistPartial", validationTaskVM, true);

                    return Json(new
                    {
                        success = true,
                        files = fileUrls,
                        partials = new
                        {
                            viewattachment = viewattachmentHtml,
                            restoration = restorationHtml
                        }
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to Update Assessment." });
                }
            }
            catch (Exception ex)
            {
                // log ex here if needed
                return Json(new { success = false, message = "Save failed. " + ex.Message });
            }
        }


        [HttpGet]
        public async Task<PartialViewResult> ViewRestorationDetails(long id)
        {
            var model = await _iIncidentService.ViewRestorationDetails(id);
            return PartialView("_ViewRestorationPartial", model);
        }

        //[HttpGet]
        //public async Task<PartialViewResult> AddRestorationDetails()
        //{
        //    ViewBag.CurrentIncidentId = Model.Id;
        //    ViewBag.CurrentIncidentValidationId = Model.incidentValidationsDetailsViewModel?.IncidentValidationId ?? 0;

        //    var model = await _iIncidentService.AddRestorationDetails();
        //    return PartialView("_AddTaskPartial", model);
        //}
        #endregion

        #region ClouseOut

        [HttpPost]
        public async Task<IActionResult> SaveClouseOutTask([FromForm] IncidentViewTaskListViewModel model)
        {
            if (model == null || model.IncidentId == 0)
                return BadRequest(new { success = false, message = "Invalid request." });

            try
            {
                // call service to add task and return the saved entity/viewmodel
                var created = await _iIncidentService.AddIncidentTaskCloseOutAsync(new AddIncidentTaskRequest
                {
                    IncidentId = model.IncidentId,
                    IncidentValidationId = model.IncidentValidationId,
                    TaskDescription = model.Task,
                    RoleIds = model.RoleIds,    // we'll pass RoleIds string in FieldValue or change model
                    StatusId = model.StatusId
                });

                // created should return a IncidentViewTaskListViewModel (or similar)
                return Ok(new { success = true, data = created });
            }
            catch (Exception ex)
            {
                // log...
                return StatusCode(500, new { success = false, message = "Save failed." });
            }
        }

        [HttpGet]
        public async Task<PartialViewResult> GetClouseOutDetails(long id)
        {
            var model = await _iIncidentService.GetvalidationTaskClouseOut(id);
            IncidentViewTaskViewModel validationTaskVM = new IncidentViewTaskViewModel
            {
                listIncidentViewTaskCloseOutViewModel = model,
            };
            return PartialView("_CloseOutChecklistPartial", validationTaskVM);
        }

        [HttpGet]
        public async Task<PartialViewResult> EditClouseOutDetails(long id)
        {
            var model = await _iIncidentService.EditClouseOutDetails(id);
            return PartialView("_UpdateCloseOutPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateClouseOut(IncidentEditTaskListViewModel model, List<IFormFile> Files)
        {
            try
            {
                var fileUrls = new List<string>();

                if (Files != null && Files.Count > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "ClouseOut");

                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    foreach (var file in Files)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(uploadsPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Relative URL for use in browser
                            var relativeUrl = $"/Storage/uploads/ClouseOut/{fileName}";
                            fileUrls.Add(relativeUrl);
                        }
                    }
                    model.ImageUrl = string.Join(",", fileUrls);
                }

                long id = await _iIncidentService.UpdateClouseOut(model);

                if (id > 0)
                {
                    AssestmentFilterRequest request = new()
                    {
                        IncidentId = model?.IncidentId ?? 0
                    };


                    var viewattachment = await _iIncidentService.ViewClouseOutAttachment(request.IncidentId);
                    var viewattachmentHtml = await this.RenderViewAsync("_ViewAttachmentAssestmentPartial", viewattachment, true);

                    var closeoutdtl = await _iIncidentService.GetvalidationTaskClouseOut(request.IncidentId);

                    IncidentViewTaskViewModel validationTaskVM = new IncidentViewTaskViewModel
                    {
                        listIncidentViewTaskCloseOutViewModel = closeoutdtl,
                    };

                    var closeoutdtlHtml = await this.RenderViewAsync("_CloseOutChecklistPartial", validationTaskVM, true);

                    return Json(new
                    {
                        success = true,
                        files = fileUrls,
                        partials = new
                        {
                            viewattachment = viewattachmentHtml,
                            closeout = closeoutdtlHtml
                        },
                        details = new
                        {
                            uploadedDocumentCount = viewattachment.Image.Count
                        }
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to Update Assessment." });
                }
            }
            catch (Exception ex)
            {
                // log ex here if needed
                return Json(new { success = false, message = "Save failed. " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<PartialViewResult> ViewClouseOutDetails(long id)
        {
            var model = await _iIncidentService.ViewClouseOutDetails(id);
            return PartialView("_ViewClouseOutPartial", model);
        }

        //[HttpGet]
        //public async Task<PartialViewResult> AddClouseOutDetails()
        //{
        //    ViewBag.CurrentIncidentId = Model.Id;
        //    ViewBag.CurrentIncidentValidationId = Model.incidentValidationsDetailsViewModel?.IncidentValidationId ?? 0;

        //    var model = await _iIncidentService.AddRestorationDetails();
        //    return PartialView("_AddTaskPartial", model);
        //}
        #endregion

        #region Repair
        [HttpGet]

        public async Task<PartialViewResult> EditRepairDetails(long id, long RepairId, long FieldType, long IncidentId, long IncidentValidationId)

        {

            var model = await _iIncidentService.EditRepairDetails(id, RepairId, FieldType, IncidentId, IncidentValidationId);

            return PartialView("_UpdateRepairPartial", model);

        }

        [HttpPost]

        public async Task<IActionResult> UpdateRepair(IncidentRepairEditViewModel model, List<IFormFile> Files)

        {

            try

            {

                var fileUrls = new List<string>();

                if (Files != null && Files.Count > 0)

                {

                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "Repair");

                    if (!Directory.Exists(uploadsPath))

                        Directory.CreateDirectory(uploadsPath);

                    foreach (var file in Files)

                    {

                        if (file.Length > 0)

                        {

                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

                            var filePath = Path.Combine(uploadsPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))

                            {

                                await file.CopyToAsync(stream);

                            }

                            // Relative URL for use in browser

                            var relativeUrl = $"/Storage/uploads/Repair/{fileName}";

                            fileUrls.Add(relativeUrl);

                        }

                    }

                    if (model.FieldTypeId == 1)

                    {

                        model.SOL_Path = string.Join(",", fileUrls);

                    }

                    else if (model.FieldTypeId == 2)

                    {

                        model.PFO_Path = string.Join(",", fileUrls);

                    }

                    else if (model.FieldTypeId == 3)

                    {

                        model.VTF_Path = string.Join(",", fileUrls);

                    }

                }

                long id = await _iIncidentService.UpdateRepair(model);

                if (id > 0)

                {

                    AssestmentFilterRequest request = new()

                    {

                        IncidentId = model?.IncidentId ?? 0

                    };

                    return Json(new

                    {

                        success = true,

                        files = fileUrls,

                    });

                }

                else

                {

                    return StatusCode(StatusCodes.Status500InternalServerError,

                        new { success = false, message = "Failed to Update Repair." });

                }

            }

            catch (Exception ex)

            {

                // log ex here if needed

                return Json(new { success = false, message = "Save failed. " + ex.Message });

            }

        }

        [HttpPost]

        public async Task<PartialViewResult> GetRepairDetails(long id)

        {

            IncidentViewModel incidentViewModel = new IncidentViewModel();

            incidentViewModel.IncidentViewRepairViewModel.listIncidentViewRepairViewModel = await _iIncidentService.GetvalidationRepairVM(id);

            return PartialView("_IncidentRepairDetailsPartial", incidentViewModel.IncidentViewRepairViewModel);

        }

        #endregion
    }
}

