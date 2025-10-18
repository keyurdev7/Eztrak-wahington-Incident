using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Repositories.Common;

using ViewModels;
using ViewModels.Incident;
using ViewModels.Policy;
using ViewModels.Users;

namespace Web.Controllers
{
    public class SettingsController : Controller
    {
        #region Init Service
        private readonly IRelationshipService<RelationshipModifyViewModel, RelationshipModifyViewModel, RelationshipDetailViewModel> _iRelationshipService;
        private readonly IEventTypeService<EventTypeModifyViewModel, EventTypeModifyViewModel, EventTypeDetailViewModel> _iEventTypeService;
        private readonly ISeverityLevelService<SeverityLevelModifyViewModel, SeverityLevelModifyViewModel, SeverityLevelDetailViewModel> _iSeverityLevelService;
        private readonly IStatusLegendService<StatusLegendModifyViewModel, StatusLegendModifyViewModel, StatusLegendDetailViewModel> _iStatusLegendService;
        private readonly IAssetIdService<AssetIdModifyViewModel, AssetIdModifyViewModel, AssetIdDetailViewModel> _iAssetIdService;
        private readonly IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel> _iAssetTypeService;
        private readonly IIncidentTeamService<IncidentTeamModifyViewModel, IncidentTeamModifyViewModel, IncidentTeamDetailViewModel> _iIncidentTeamService;
        private readonly IPolicyService<PolicyModifyViewModel, PolicyModifyViewModel, PolicyDetailViewModel> _iPolicyService;
        private readonly IUserManagementService<UserManagementModifyViewModel, UserManagementModifyViewModel, UserDetailViewModel> _iUserManagementService;
        private readonly IUsersinService<UserModifyViewModel, UserModifyViewModel, UserDetailViewModel> _iUsersinService;
        private readonly IProgressService<ProgressModifyViewModel, ProgressModifyViewModel, ProgressDetailViewModel> _iProgressService;
        private readonly IMaterialService<MaterialModifyViewModel, MaterialModifyViewModel, MaterialDetailViewModel> _iMaterialService;
        private readonly IEquipmentFieldsService<EquipmentFieldsModifyViewModel, EquipmentFieldsModifyViewModel, EquipmentFieldsDetailViewModel> _iEquipmentFieldsService;
        private readonly IIncidentRoleService<IncidentRoleModifyViewModel, IncidentRoleModifyViewModel, IncidentRoleDetailViewModel> _iIncidentRoleService;
        private readonly ICompanyService<CompanyModifyViewModel, CompanyModifyViewModel, CompanyDetailViewModel> _iCompanyService;
        private readonly IIncidentShiftService<IncidentShiftModifyViewModel, IncidentShiftModifyViewModel, IncidentShiftDetailViewModel> _iIncidentShiftService;
        #endregion

        #region Ctor
        public SettingsController(IRelationshipService<RelationshipModifyViewModel, RelationshipModifyViewModel, RelationshipDetailViewModel> iRelationshipService, IEventTypeService<EventTypeModifyViewModel, EventTypeModifyViewModel, EventTypeDetailViewModel> iEventTypeService, ISeverityLevelService<SeverityLevelModifyViewModel, SeverityLevelModifyViewModel, SeverityLevelDetailViewModel> iSeverityLevelService, IStatusLegendService<StatusLegendModifyViewModel, StatusLegendModifyViewModel, StatusLegendDetailViewModel> iStatusLegendService, IAssetIdService<AssetIdModifyViewModel, AssetIdModifyViewModel, AssetIdDetailViewModel> iAssetIdService,
            IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeDetailViewModel> iAssetTypeService, IIncidentTeamService<IncidentTeamModifyViewModel, IncidentTeamModifyViewModel, IncidentTeamDetailViewModel> iIncidentTeamService,
            IUserManagementService<UserManagementModifyViewModel, UserManagementModifyViewModel, UserDetailViewModel> iUserManagementService,
            IPolicyService<PolicyModifyViewModel, PolicyModifyViewModel, PolicyDetailViewModel> iPolicyService, IUsersinService<UserModifyViewModel, UserModifyViewModel, UserDetailViewModel> iusersinService,
            IProgressService<ProgressModifyViewModel, ProgressModifyViewModel, ProgressDetailViewModel> iProgressService, IMaterialService<MaterialModifyViewModel, MaterialModifyViewModel, MaterialDetailViewModel> iMaterialService, IEquipmentFieldsService<EquipmentFieldsModifyViewModel, EquipmentFieldsModifyViewModel, EquipmentFieldsDetailViewModel> iEquipmentFieldsService, IIncidentRoleService<IncidentRoleModifyViewModel, IncidentRoleModifyViewModel, IncidentRoleDetailViewModel> iIncidentRoleService,
            ICompanyService<CompanyModifyViewModel, CompanyModifyViewModel, CompanyDetailViewModel> iCompanyService, IIncidentShiftService<IncidentShiftModifyViewModel, IncidentShiftModifyViewModel, IncidentShiftDetailViewModel> iIncidentShiftService)
        {
            _iRelationshipService = iRelationshipService;
            _iEventTypeService = iEventTypeService;
            _iSeverityLevelService = iSeverityLevelService;
            _iStatusLegendService = iStatusLegendService;
            _iAssetIdService = iAssetIdService;
            _iAssetTypeService = iAssetTypeService;
            _iIncidentTeamService = iIncidentTeamService;
            _iUserManagementService = iUserManagementService;
            _iPolicyService = iPolicyService;
            _iUsersinService = iusersinService;
            _iProgressService = iProgressService;
            _iEquipmentFieldsService = iEquipmentFieldsService;
            _iCompanyService = iCompanyService;
            _iMaterialService = iMaterialService;
            _iIncidentRoleService = iIncidentRoleService;
            _iIncidentShiftService = iIncidentShiftService;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        #region Source
        [HttpGet]
        public async Task<IActionResult> GetAllRelationships()
        {
            var model = await _iRelationshipService.GetAllRelationships();
            return PartialView("~/Views/Settings/Source/_ListSource.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddRelationships()
        {
            var model = new RelationshipModifyViewModel();
            return PartialView("~/Views/Settings/Source/_AddSource.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetRelationshipById(long id)
        {
            var model = await _iRelationshipService.GetRelationById(id);
            return PartialView("~/Views/Settings/Source/_AddSource.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveRelation([FromForm] RelationshipModifyViewModel relationship)
        {
            if (relationship == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (relationship.Id > 0)
                {
                    Id = await _iRelationshipService.UpdateRelation(relationship);
                }
                else
                {
                    Id = await _iRelationshipService.SaveRelation(relationship);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save relation." });

                var successMsg = $"Source saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRelationshipById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iRelationshipService.DeleteRelation(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete relation." });

                var successMsg = $"Source deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region EventType
        [HttpGet]
        public async Task<IActionResult> GetAllEventTypes()
        {
            var model = await _iEventTypeService.GetAllEventTypes();
            return PartialView("~/Views/Settings/EventType/_ListEventType.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddEventType()
        {
            var model = new EventTypeModifyViewModel();
            return PartialView("~/Views/Settings/EventType/_AddEventType.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventTypeById(long id)
        {
            var model = await _iEventTypeService.GetEventTypeById(id);
            return PartialView("~/Views/Settings/EventType/_AddEventType.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEventType([FromForm] EventTypeModifyViewModel eventType)
        {
            if (eventType == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (eventType.Id > 0)
                {
                    Id = await _iEventTypeService.UpdateEventType(eventType);
                }
                else
                {
                    Id = await _iEventTypeService.SaveEventType(eventType);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save event type." });

                var successMsg = $"Event type saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteEventTypeById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iEventTypeService.DeleteEventType(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete relation." });

                var successMsg = $"Event Type deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region SeverityLevel
        [HttpGet]
        public async Task<IActionResult> GetAllSeverity()
        {
            var model = await _iSeverityLevelService.GetAllSeverityLevels();
            return PartialView("~/Views/Settings/SeverityLevel/_ListSeverityLevel.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddSeverity()
        {
            var model = new SeverityLevelModifyViewModel();
            return PartialView("~/Views/Settings/SeverityLevel/_AddSeverityLevel.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSeverityById(long id)
        {
            var model = await _iSeverityLevelService.GetSeverityLevelById(id);
            return PartialView("~/Views/Settings/SeverityLevel/_AddSeverityLevel.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSeverity([FromForm] SeverityLevelModifyViewModel severityLevel)
        {
            if (severityLevel == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (severityLevel.Id > 0)
                {
                    Id = await _iSeverityLevelService.UpdateSeverityLevel(severityLevel);
                }
                else
                {
                    Id = await _iSeverityLevelService.SaveSeverityLevel(severityLevel);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save severity level." });

                var successMsg = $"Severity level saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSeverityById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iSeverityLevelService.DeleteSeverityLevel(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete severity level." });

                var successMsg = $"Severity level deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region StatusLegend

        [HttpGet]
        public async Task<IActionResult> GetAllStatusLegend()
        {
            var model = await _iStatusLegendService.GetAllStatusLegends();
            return PartialView("~/Views/Settings/StatusLegend/_ListStatusLegend.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddStatusLegend()
        {
            var model = new StatusLegendModifyViewModel();
            return PartialView("~/Views/Settings/StatusLegend/_AddStatusLegend.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatusLegendById(long id)
        {
            var model = await _iStatusLegendService.GetStatusLegendById(id);
            return PartialView("~/Views/Settings/StatusLegend/_AddStatusLegend.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveStatusLegend([FromForm] StatusLegendModifyViewModel statusLegend)
        {
            if (statusLegend == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (statusLegend.Id > 0)
                {
                    Id = await _iStatusLegendService.UpdateStatusLegend(statusLegend);
                }
                else
                {
                    Id = await _iStatusLegendService.SaveStatusLegend(statusLegend);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save status legend." });

                var successMsg = $"Status legend saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteStatusLegendById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iStatusLegendService.DeleteStatusLegend(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete status legend." });

                var successMsg = $"Status legend deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region AssetIds
        [HttpGet]
        public async Task<IActionResult> GetAllAssetIds()
        {
            var model = await _iAssetIdService.GetAllAssetIds();
            return PartialView("~/Views/Settings/AssetId/_ListAssetId.cshtml", model);
        }
        // inside SettingsController

        [HttpGet]
        public async Task<IActionResult> AddAssetId()
        {
            var model = new AssetIdModifyViewModel();
            return PartialView("~/Views/Settings/AssetId/_AddAssetId.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetIdById(long id)
        {
            var model = await _iAssetIdService.GetAssetIdById(id);
            return PartialView("~/Views/Settings/AssetId/_AddAssetId.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAssetId([FromForm] AssetIdModifyViewModel asset)
        {
            if (asset == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long id = 0;
                if (asset.Id > 0)
                    id = await _iAssetIdService.UpdateAssetId(asset);
                else
                    id = await _iAssetIdService.SaveAssetId(asset);

                if (id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save asset." });

                var successMsg = "Asset saved successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssetIdById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var deletedId = await _iAssetIdService.DeleteAssetId(id);

                if (deletedId == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete asset." });

                var successMsg = "Asset deleted successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region AssetTypes
        [HttpGet]
        public async Task<IActionResult> GetAllAssetTypes()
        {
            var model = await _iAssetTypeService.GetAllAssetTypes();
            return PartialView("~/Views/Settings/AssetType/_ListAssetType.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddAssetType()
        {
            var model = new AssetTypeModifyViewModel();
            return PartialView("~/Views/Settings/AssetType/_AddAssetType.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetTypeById(long id)
        {
            var model = await _iAssetTypeService.GetAssetTypeById(id);
            return PartialView("~/Views/Settings/AssetType/_AddAssetType.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAssetType([FromForm] AssetTypeModifyViewModel assetType)
        {
            if (assetType == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long id = 0;
                if (assetType.Id > 0)
                    id = await _iAssetTypeService.UpdateAssetType(assetType);
                else
                    id = await _iAssetTypeService.SaveAssetType(assetType);

                if (id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save asset type." });

                return Ok(new { success = true, data = "Asset type saved successfully!" });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssetTypeById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var deletedId = await _iAssetTypeService.DeleteAssetType(id);

                if (deletedId == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete asset type." });

                return Ok(new { success = true, data = "Asset type deleted successfully!" });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion
        #region IncidentTeams
        [HttpGet]
        public async Task<IActionResult> GetAllIncidentTeams()
        {
            var model = await _iIncidentTeamService.GetAllIncidentTeams();
            return PartialView("~/Views/Settings/IncidentTeam/_ListIncidentTeam.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddIncidentTeam()
        {
            var model = new IncidentTeamModifyViewModel();
            return PartialView("~/Views/Settings/IncidentTeam/_AddIncidentTeam.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetIncidentTeamById(long id)
        {
            var model = await _iIncidentTeamService.GetIncidentTeamById(id);
            return PartialView("~/Views/Settings/IncidentTeam/_AddIncidentTeam.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveIncidentTeam([FromForm] IncidentTeamModifyViewModel incidentTeam)
        {
            if (incidentTeam == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long id = 0;
                if (incidentTeam.Id > 0)
                    id = await _iIncidentTeamService.UpdateIncidentTeam(incidentTeam);
                else
                    id = await _iIncidentTeamService.SaveIncidentTeam(incidentTeam);

                if (id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save incident team." });

                var successMsg = "Incident team saved successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteIncidentTeamById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var deletedId = await _iIncidentTeamService.DeleteIncidentTeam(id);

                if (deletedId == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete incident team." });

                var successMsg = "Incident team deleted successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion
        #region Policy
        [HttpGet]
        public async Task<IActionResult> GetAllPolicies()
        {
            var model = await _iPolicyService.GetAllPolicies();
            return PartialView("~/Views/Settings/Policy/_ListPolicy.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddPolicy()
        {
            var model = new PolicyModifyViewModel();
            return PartialView("~/Views/Settings/Policy/_AddPolicy.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPolicyById(long id)
        {
            var model = await _iPolicyService.GetPolicyById(id);
            return PartialView("~/Views/Settings/Policy/_AddPolicy.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SavePolicy([FromForm] PolicyModifyViewModel policy)
        {
            if (policy == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                // ensure PolicySteps is populated from the incoming form string (hidden input)
                var stepsRaw = Request.Form["PolicySteps"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(stepsRaw))
                {
                    policy.PolicySteps = stepsRaw
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                }
                else
                {
                    policy.PolicySteps = new List<string>();
                }

                long Id = 0;
                if (policy.Id > 0)
                {
                    Id = await _iPolicyService.UpdatePolicy(policy);
                }
                else
                {
                    Id = await _iPolicyService.SavePolicy(policy);
                }

                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save policy." });

                var successMsg = $"Policy saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeletePolicyById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iPolicyService.DeletePolicy(id);
                }

                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete policy." });

                var successMsg = $"Policy deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPolicySteps([FromBody] AddPolicyStepsRequest req)
        {
            if (req == null || req.PolicyId <= 0 || req.Steps == null || !req.Steps.Any())
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var updatedId = await _iPolicyService.AddPolicySteps(req.PolicyId, req.Steps);
                if (updatedId == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Failed to add procedures." });

                return Ok(new { success = true, data = "Procedures added successfully." });
            }
            catch (Exception ex)
            {
                // _logger?.LogError(ex, "Error AddPolicySteps");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region UserManagement

        [HttpGet]
        public async Task<IActionResult> GetAllUserManagement()
        {
            var model = await _iUserManagementService.GetAllUserManagement();
            return PartialView("~/Views/Settings/UserManagement/_ListUserManagement.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserManagement()
        {
            var model = new UserManagementModifyViewModel();
            var roles = await _iUserManagementService.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
            return PartialView("~/Views/Settings/UserManagement/_AddUserManagement.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserManagementById(long id)
        {
            var roles = await _iUserManagementService.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "RoleId", "RoleName");
            var model = await _iUserManagementService.GetUserManagementById(id);
            return PartialView("~/Views/Settings/UserManagement/_AddUserManagement.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserManagement([FromForm] UserManagementModifyViewModel User)
        {
            if (User == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long id = 0;
                if (User.Id > 0)
                    id = await _iUserManagementService.UpdateUserManagement(User);
                else
                    id = await _iUserManagementService.SaveUserManagement(User);

                if (id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save usermanagement.Use diffrent email address." });

                var successMsg = "UserManagement saved successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserManagementById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var deletedId = await _iUserManagementService.DeleteUserManagement(id);

                if (deletedId == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete usermanagement." });

                var successMsg = "UserManagement deleted successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var model = await _iUsersinService.GetAllUsers();
            return PartialView("~/Views/Settings/Users/_ListUser.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var model = new UserModifyViewModel();
            var teams = await _iUsersinService.GetAllTeams();
            ViewBag.Teams = new SelectList(teams, "TeamId", "TeamName");
            var companies = await _iUsersinService.GetAllCompanies();
            ViewBag.Companies = new SelectList(companies, "CompanyId", "CompanyName");
            var roles = await _iUsersinService.GetAllIncidentRoles();
            ViewBag.Roles = new SelectList(roles, "IncidentRoleId", "IncidentRoleName");
            return PartialView("~/Views/Settings/Users/_AddUser.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(long id)
        {
            var teams = await _iUsersinService.GetAllTeams();
            ViewBag.Teams = new SelectList(teams, "TeamId", "TeamName");
            var companies = await _iUsersinService.GetAllCompanies();
            ViewBag.Companies = new SelectList(companies, "CompanyId", "CompanyName");
            var roles = await _iUsersinService.GetAllIncidentRoles();
            ViewBag.Roles = new SelectList(roles, "IncidentRoleId", "IncidentRoleName");
            var model = await _iUsersinService.GetUserById(id);
            return PartialView("~/Views/Settings/Users/_AddUser.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser([FromForm] UserModifyViewModel User)
        {
            if (User == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long id = 0;
                if (User.Id > 0)
                    id = await _iUsersinService.UpdateUser(User);
                else
                    id = await _iUsersinService.SaveUser(User);

                if (id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save user." });

                var successMsg = "User saved successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var deletedId = await _iUsersinService.DeleteUser(id);

                if (deletedId == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete user." });

                var successMsg = "User deleted successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region Progress
        [HttpGet]
        public async Task<IActionResult> GetAllProgress()
        {
            var model = await _iProgressService.GetAllProgress();
            return PartialView("~/Views/Settings/Progress/_ListProgress.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddProgress()
        {
            var model = new ProgressModifyViewModel();
            return PartialView("~/Views/Settings/Progress/_AddProgress.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetProgressById(long id)
        {
            var model = await _iProgressService.GetProgressById(id);
            return PartialView("~/Views/Settings/Progress/_AddProgress.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProgress([FromForm] ProgressModifyViewModel progress)
        {
            if (progress == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (progress.Id > 0)
                {
                    Id = await _iProgressService.UpdateProgress(progress);
                }
                else
                {
                    Id = await _iProgressService.SaveProgress(progress);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save status master." });

                var successMsg = $"Status Master saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProgressById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iProgressService.DeleteProgress(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete status master." });

                var successMsg = $"Status Master deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        #endregion
        // In appropriate Controller (e.g., SettingsController)
        #region Materials

        [HttpGet]
        public async Task<IActionResult> GetAllMaterials()
        {
            var model = await _iMaterialService.GetAllMaterials();
            return PartialView("~/Views/Settings/Material/_ListMaterial.cshtml", model);
        }

        [HttpGet]
        public IActionResult AddMaterial()
        {
            var model = new MaterialModifyViewModel();
            return PartialView("~/Views/Settings/Material/_AddMaterial.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialById(long id)
        {
            var model = await _iMaterialService.GetMaterialById(id);
            return PartialView("~/Views/Settings/Material/_AddMaterial.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMaterial([FromForm] MaterialModifyViewModel material)
        {
            if (material == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long id = 0;
                if (material.Id > 0)
                    id = await _iMaterialService.UpdateMaterial(material);
                else
                    id = await _iMaterialService.SaveMaterial(material);

                if (id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save material." });

                var successMsg = "Material saved successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMaterialById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var deletedId = await _iMaterialService.DeleteMaterial(id);

                if (deletedId == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete material." });

                var successMsg = "Material deleted successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion


        #region EquipmentFields

        [HttpGet]
        public async Task<IActionResult> GetAllEquipmentFields()
        {
            var model = await _iEquipmentFieldsService.GetAllEquipmentFields();
            return PartialView("~/Views/Settings/EquipmentFields/_ListEquipmentFields.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddEquipmentFields()
        {
            var model = new EquipmentFieldsModifyViewModel();
            return PartialView("~/Views/Settings/EquipmentFields/_AddEquipmentFields.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipmentFieldsById(long id)
        {
            var model = await _iEquipmentFieldsService.GetEquipmentFieldsById(id);
            return PartialView("~/Views/Settings/EquipmentFields/_AddEquipmentFields.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEquipmentFields([FromForm] EquipmentFieldsModifyViewModel equipmentfields)
        {
            if (equipmentfields == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (equipmentfields.Id > 0)
                {
                    Id = await _iEquipmentFieldsService.UpdateEquipmentFields(equipmentfields);
                }
                else
                {
                    Id = await _iEquipmentFieldsService.SaveEquipmentFields(equipmentfields);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save Equipment Field." });

                var successMsg = $"Equipment Field saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteEquipmentFieldsById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iEquipmentFieldsService.DeleteEquipmentFields(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete equipment field." });

                var successMsg = $"Equipment Field deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        #endregion
        #region IncidentRole
        [HttpGet]
        public async Task<IActionResult> GetAllIncidentRoles()
        {
            var model = await _iIncidentRoleService.GetAllIncidentRoles();
            return PartialView("~/Views/Settings/IncidentRole/_ListIncidentRole.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddIncidentRole()
        {
            var model = new IncidentRoleModifyViewModel();
            return PartialView("~/Views/Settings/IncidentRole/_AddIncidentRole.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetIncidentRoleById(long id)
        {
            var model = await _iIncidentRoleService.GetIncidentRoleById(id);
            return PartialView("~/Views/Settings/IncidentRole/_AddIncidentRole.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveIncidentRole([FromForm] IncidentRoleModifyViewModel incidentRole)
        {
            if (incidentRole == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (incidentRole.Id > 0)
                {
                    Id = await _iIncidentRoleService.UpdateIncidentRole(incidentRole);
                }
                else
                {
                    Id = await _iIncidentRoleService.SaveIncidentRole(incidentRole);
                }

                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save incident role." });

                var successMsg = $"Incident role saved successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        #endregion

        #region Company
        [HttpGet]
        public async Task<IActionResult> GetAllCompany()
        {
            var model = await _iCompanyService.GetAllCompanys();
            return PartialView("~/Views/Settings/Company/_ListCompany.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddCompany()
        {
            var model = new CompanyModifyViewModel();
            return PartialView("~/Views/Settings/Company/_AddCompany.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyById(long id)
        {
            var model = await _iCompanyService.GetCompanyById(id);
            return PartialView("~/Views/Settings/Company/_AddCompany.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCompany([FromForm] CompanyModifyViewModel company)
        {
            if (company == null)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (company.Id > 0)
                {
                    Id = await _iCompanyService.UpdateCompany(company);
                }
                else
                {
                    Id = await _iCompanyService.SaveCompany(company);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to save company." });

                var successMsg = $"Company saved successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCompanyById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iCompanyService.DeleteCompany(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete company." });

                var successMsg = $"Company deleted successfully!";

                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteIncidentRoleById(long id)
        {
            if (id == 0)
                return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (id > 0)
                {
                    Id = await _iIncidentRoleService.DeleteIncidentRole(id);
                }
                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { success = false, message = "Failed to delete incident role." });

                var successMsg = $"Incident role deleted successfully!";
                return Ok(new { success = true, data = successMsg });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion
        #region IncidentShift
        [HttpGet]
        public async Task<IActionResult> GetAllIncidentShifts()
        {
            var model = await _iIncidentShiftService.GetAllIncidentShifts();
            return PartialView("~/Views/Settings/IncidentShift/_ListIncidentShift.cshtml", model);
        }

        [HttpGet]
        public IActionResult AddIncidentShift()
        {
            var model = new IncidentShiftModifyViewModel();
            return PartialView("~/Views/Settings/IncidentShift/_AddIncidentShift.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetIncidentShiftById(long id)
        {
            var model = await _iIncidentShiftService.GetIncidentShiftById(id);
            return PartialView("~/Views/Settings/IncidentShift/_AddIncidentShift.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveIncidentShift([FromForm] IncidentShiftModifyViewModel incidentShift)
        {
            if (incidentShift == null) return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                long Id = 0;
                if (incidentShift.Id > 0)
                    Id = await _iIncidentShiftService.UpdateIncidentShift(incidentShift);
                else
                    Id = await _iIncidentShiftService.SaveIncidentShift(incidentShift);

                if (Id == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Failed to save incident shift." });

                return Ok(new { success = true, data = "Incident shift saved successfully!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteIncidentShiftById(long id)
        {
            if (id == 0) return BadRequest(new { success = false, message = "Invalid request data." });

            try
            {
                var Id = await _iIncidentShiftService.DeleteIncidentShift(id);
                if (Id == 0) return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Failed to delete incident shift." });

                return Ok(new { success = true, data = "Incident shift deleted successfully!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> GetCompanyAndRole(long userId)
        {
            if (userId == 0)
                return Json(new { userId = 0, roleId = 0, companyId = 0 });

            var result = await _iUsersinService.GetUserById(userId);

            if (result is null)
                return Json(new { userId = 0, roleId = 0, companyId = 0 });

            return Json(new
            {
                userId = result.Id,
                roleId = result.IncidentRoleId,
                companyId = result.CompanyId
            });
        }
    }
}