using AutoMapper;

using Azure;

using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

using Enums;

using Helpers.Extensions;
using Helpers.File;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Shared.UserInfoServices.Interface;

using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.Incident;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class IncidentValidationService : IIncidentValidationService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IncidentValidationService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdditionalLocationsService _iAdditionalLocationsService;

        public IncidentValidationService(ApplicationDbContext db, ILogger<IncidentValidationService> logger,
                                        IHttpContextAccessor httpContextAccessor, IAdditionalLocationsService iAdditionalLocationsService)
        {
            _db = db;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _iAdditionalLocationsService = iAdditionalLocationsService;
        }

        public async Task<List<IncidentValidationPendingViewModel>> GetValidationPendingList()
        {
            List<IncidentValidationPendingViewModel> incidentValidationPendings = new();
            try
            {
                var query = _db.Incidents.Where(p => !p.IsDeleted && p.StatusLegendId != (int)StatusLegendEnum.Validated)
                             .Include(p => p.SeverityLevel)
                             .AsQueryable();

                var incidentsList = await query.ToListAsync();


                foreach (var item in incidentsList)
                {
                    incidentValidationPendings.Add(new IncidentValidationPendingViewModel()
                    {
                        EventType = await GetEventTypes(item.EventTypeIds ?? string.Empty),
                        Id = item.Id,
                        Severity = item.SeverityLevel.Name,
                        SeverityColor = item.SeverityLevel.Color,
                        Description = item?.DescriptionIssue ?? string.Empty,
                        IncidentId = item.IncidentID,
                        IncidentLocation = item.LocationAddress ?? string.Empty,
                        IncidentDate = GetDate(Convert.ToString(item.CallTime))
                    });
                }
                return incidentValidationPendings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetValidationPendingList.");
                return new List<IncidentValidationPendingViewModel>();
            }
        }

        public async Task<List<RecentlyIncidentValidationViewModel>> GetRecentlyValidationList()
        {
            List<RecentlyIncidentValidationViewModel> incidentValidationPendings = new();
            try
            {
                var query = _db.Incidents.Where(p => !p.IsDeleted && p.StatusLegendId == (int)StatusLegendEnum.Validated
                             && p.UpdatedOn == DateTime.Today)
                             .Include(p => p.SeverityLevel)
                             .AsQueryable();

                var incidentsList = await query.ToListAsync();

                foreach (var item in incidentsList)
                {
                    incidentValidationPendings.Add(new RecentlyIncidentValidationViewModel()
                    {
                        EventType = await GetEventTypes(item.EventTypeIds ?? string.Empty),
                        Id = item.Id,
                        Status = item.StatusLegend.Name,
                        StatusColor = item.StatusLegend.Color,
                        IncidentId = item.IncidentID,
                        IncidentDate = GetDate(Convert.ToString(item.CallTime))
                    });
                }
                return incidentValidationPendings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetRecentlyValidationList.");
                return new List<RecentlyIncidentValidationViewModel>();
            }
        }

        public async Task<long> GetHighPriorityIncidentCount()
        {
            try
            {
                return await _db.Incidents
                    .Where(p => !p.IsDeleted
                                && p.StatusLegendId != (int)StatusLegendEnum.Validated
                                && p.SeverityLevel.Name == SeverityEnum.High.ToString())
                    .LongCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetHighPriorityIncidentCount.");
                return 0;
            }
        }

        public async Task<IncidentValidationDetailViewModel> GetIncidentValidationDetail(long id)
        {
            try
            {
                var incident = await _db.Incidents
                    .Include(p => p.SeverityLevel)
                    .Include(p => p.StatusLegend)
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

                if (incident == null)
                {
                    return new IncidentValidationDetailViewModel();
                }

                // Run async calls in parallel
                var eventTypesTask = await GetEventTypes(incident.EventTypeIds ?? string.Empty);
                var assetsTask = await GetAssets(incident.AssetIds ?? string.Empty);


                return new IncidentValidationDetailViewModel
                {
                    Id = incident.Id,
                    CallerAddress = incident.CallerAddress ?? string.Empty,
                    CallerContact = incident.CallerPhoneNumber ?? string.Empty,
                    CallerDateTime = GetDate(incident.CallTime.ToString()),
                    CallerName = incident.CallerName ?? string.Empty,
                    EventType = eventTypesTask,
                    IncidentId = incident.IncidentID,
                    IncidentLocation = incident.LocationAddress ?? string.Empty,
                    NearestIntersection = incident.Landmark ?? string.Empty,
                    AffectedAssets = assetsTask,
                    Lat = incident.Lat,
                    Long = incident.Lng,
                    IncidentStatus = incident.StatusLegend?.Name ?? string.Empty,
                    IncidentStatusColor = incident.StatusLegend?.Color ?? string.Empty,
                    Severity = incident.SeverityLevel?.Name ?? string.Empty,
                    SeverityColor = incident.SeverityLevel?.Color ?? string.Empty,
                    DescriptionIssue = incident.DescriptionIssue ?? string.Empty,
                    EvacuationRequired = GetIndicator(incident.EvacuationRequiredId),
                    GasPresent = GetIndicator(incident.GasPresentId),
                    HissingPresent = GetIndicator(incident.HissingPresentId),
                    PeopleInjured = GetIndicator(incident.PeopleInjuredId),
                    VisibleDamagePresent = GetIndicator(incident.VisibleDamagePresentId),
                    WaterPresent = GetIndicator(incident.WaterPresentId),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetIncidentValidationDetail.");
                return new IncidentValidationDetailViewModel();
            }
        }

        public async Task<IncidentValidationViewModel> GetIncidentValidationAlarm(long id)
        {
            try
            {
                var incidentTask = await _db.Incidents
                    .Include(p => p.SeverityLevel)
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

                var severityLevelsTask = await _db.SeverityLevels
                                   .Where(it => !it.IsDeleted)
                                   .OrderBy(it => it.Name == "High" ? 1 :
                                                  it.Name == "Moderate" ? 2 :
                                                  it.Name == "Low" ? 3 : 4)
                                   .Select(it => new SelectListItem
                                   {
                                       Value = it.Id.ToString(),
                                       Text = !string.IsNullOrWhiteSpace(it.Description)
                                              ? it.Name + " (" + it.Description + ")"
                                              : it.Name
                                   })
                                   .ToListAsync();

                var UserLisTTask = await _db.IncidentUsers
                                   .Where(it => !it.IsDeleted)
                                   .Select(it => new SelectListItem
                                   {
                                       Value = it.Id.ToString(),
                                       Text = it.FirstName + ' ' + it.LastName
                                   })
                                   .ToListAsync();


                var companyList = await _db.Company
                                   .Where(it => !it.IsDeleted)
                                   .Select(it => new SelectListItem
                                   {
                                       Value = it.Id.ToString(),
                                       Text = it.Name
                                   })
                                   .ToListAsync();


                var rolesList = await _db.IncidentRoles
                                   .Where(it => !it.IsDeleted)
                                   .Select(it => new SelectListItem
                                   {
                                       Value = it.Id.ToString(),
                                       Text = it.Name
                                   })
                                   .ToListAsync();

                var shiftsList = await _db.IncidentShifts
                                   .Where(it => !it.IsDeleted)
                                   .Select(it => new SelectListItem
                                   {
                                       Value = it.Id.ToString(),
                                       Text = it.Name
                                   })
                                   .ToListAsync();

                var statusList = await _db.Progress
                                  .Where(it => !it.IsDeleted)
                                  .Select(it => new SelectListItem
                                  {
                                      Value = it.Id.ToString(),
                                      Text = it.Name
                                  })
                                  .ToListAsync();

                if (incidentTask == null)
                {
                    return new IncidentValidationViewModel { severityLevels = severityLevelsTask };
                }

                return new IncidentValidationViewModel
                {
                    Id = incidentTask.Id,
                    IncidentId = incidentTask.IncidentID,
                    IncidentLocation = incidentTask.LocationAddress ?? string.Empty,
                    severityLevels = severityLevelsTask,
                    UserList = UserLisTTask,
                    severityLevel = incidentTask.SeverityLevel?.Name ?? string.Empty,
                    Lat = incidentTask.Lat,
                    Long = incidentTask.Lng,
                    CompanyList = companyList,
                    RoleList = rolesList,
                    ShiftsList = shiftsList,
                    StatusList = statusList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetIncidentValidationAlarm.");
                return new IncidentValidationViewModel();
            }
        }

        public async Task<List<IncidentResponseTeamViewModel>> GetIncidentValidationResponseTeam()
        {
            List<IncidentResponseTeamViewModel> incidentResponseTeams = new();

            try
            {
                var responseTeams = await _db.IncidentTeams.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var item in responseTeams)
                {
                    incidentResponseTeams.Add(new IncidentResponseTeamViewModel()
                    {
                        ReponseTeamId = item.Id,
                        Name = item.Name,
                        Contact = item.Contact ?? string.Empty,
                        Specializations = item.Specializations ?? string.Empty
                    });
                }

                return incidentResponseTeams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetIncidentValidationResponseTeam.");
                return new List<IncidentResponseTeamViewModel>();
            }
        }

        public async Task<List<IncidentPolicyViewModel>> GetIncidentValidationPolicy()
        {
            List<IncidentPolicyViewModel> incidentPolicies = new();

            try
            {
                var assignTeams = await _db.IncidentTeams.Where(p => !p.IsDeleted).ToListAsync();

                var policies = await _db.Policies.Where(p => !p.IsDeleted).ToListAsync();

                foreach (var item in policies)
                {
                    incidentPolicies.Add(new IncidentPolicyViewModel()
                    {
                        PolicyId = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        assignTeams = assignTeams.Select(p => new SelectListItem()
                        {
                            Text = p.Name,
                            Value = p.Id.ToString()
                        }).ToList(),
                        PolicySteps = item.PolicySteps?.Split(',').ToList() ?? new List<string>()
                    });
                }

                return incidentPolicies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetIncidentValidationPolicy.");
                return new List<IncidentPolicyViewModel>();
            }
        }

        public async Task<long> SavePolicy(PolicyModifyViewModel request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Map ViewModel → Entity
                var policy = new Policy
                {
                    Name = request.Name,
                    Description = request.Description ?? string.Empty
                };

                // Save
                await _db.Policies.AddAsync(policy);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return policy.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SavePolicy.");
                return 0;
            }
        }

        public async Task<List<SelectListItem>> GetTeamsList()
        {
            try
            {
                var assignResponses = await _db.IncidentTeams.Where(p => !p.IsDeleted)
                             .ToListAsync();
                var assignResponseTeams = assignResponses.Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();

                return assignResponseTeams;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetTeamsList.");
                return new List<SelectListItem>();
            }
        }

        public async Task<long> SaveIncidentValidation(IncidentSubmitViewModel request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                #region IncidentValidation
                // 1. Save main IncidentValidation
                var incidentValidation = new IncidentValidation
                {
                    IncidentId = request.Id,
                    IsMarkFalseAlarm = false,
                    ValidationNotes = request.ValidationNotes ?? "test",
                    AssignResponseTeams = "1",//request.AssignResponseTeams,
                    ConfirmedSeverityLevelId = request.ConfirmedSeverityLevelId,
                    DiscoveryPerimeterId = request.DiscoveryPerimeterId,
                };

                await _db.IncidentValidations.AddAsync(incidentValidation);
                await _db.SaveChangesAsync();

                //var insertedValidationId = incidentValidation.Id;
                #endregion

                // 4. Add Incident Validation Locations
                if (request.listSubmitValidationLocationVM.Any())
                {
                    var validationLocation = request.listSubmitValidationLocationVM.Select(item =>
                    {
                        return new IncidentValidationLocation
                        {
                            IncidentId = request.Id,
                            IncidentValidationId = incidentValidation.Id,
                            ICPLocation = item.ICPLocation ?? string.Empty,
                            DiscoveryPerimeterId = item.DiscoveryPerimeter ?? 0,
                            Lat = item.Lat,
                            Lng = item.Lon,
                            Source = item.Source,
                            ConfirmedSeverityLevelId = item.SeverityID ?? 0,
                            AdditionalLocationId = item.LocationId
                        };
                    }).ToList();
                    await _db.IncidentValidationLocations.AddRangeAsync(validationLocation);
                }

                // 6. Save main IncidentValidationAssignedRole
                var IncidentValidationAssignedRole = new IncidentValidationAssignedRole
                {
                    IncidentValidationId = incidentValidation.Id,
                    IncidentId = request.Id,
                    IncidentCommander = request.assignedRole.IncidentCommanderId,
                    FieldEnvRep = request.assignedRole.FieldEnvRepId,
                    GEC_Coordinator = request.assignedRole.GECCoordinatorId,
                    EngineeringLead = request.assignedRole.EngineeringLeadId,
                    ActiveStatus = ActiveStatus.Active
                };
                await _db.IncidentValidationAssignedRoles.AddAsync(IncidentValidationAssignedRole);

                // 7. Save main IncidentValidationAssignedRole
                var IncidentValidationGate = new IncidentValidationGate
                {
                    IncidentValidationId = incidentValidation.Id,
                    IncidentId = request.Id,
                    ContainmentAcknowledgement = request.validationGates.ContainmentAcknowledgement,
                    Exception = request.validationGates.Exception,
                    IndependentInspection = request.validationGates.IndependentInspection,
                    Regulatory = request.validationGates.Regulatory,
                    IsOtherEvent = request.validationGates.IsOtherEvent,
                    OtherEventDetail = request.validationGates.OtherEventDetail,
                    ActiveStatus = ActiveStatus.Active
                };
                await _db.IncidentValidationGates.AddAsync(IncidentValidationGate);

                #region Update Incident record
                // 5. Update Incident record
                var incident = await _db.Incidents.FirstOrDefaultAsync(p => p.Id == request.Id);
                if (incident != null)
                {
                    var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userIdParsed = !string.IsNullOrEmpty(userId) ? long.Parse(userId) : 0;

                    // ⚠️ Replace with real logged-in user
                    var statusLegend = await _db.StatusLegends.FirstOrDefaultAsync(x => x.Name == StatusLegendEnum.Validated.ToString());

                    incident.StatusLegendId = statusLegend?.Id ?? (int)StatusLegendEnum.Validated;
                    //incident.SeverityLevelId = (int)SeverityEnum.Low;
                    incident.UpdatedOn = DateTime.Now;
                    incident.UpdatedBy = userIdParsed;
                }

                #endregion
                var NuserId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var NuserIdParsed = !string.IsNullOrEmpty(NuserId) ? long.Parse(NuserId) : 0;
                var userName = _httpContextAccessor?.HttpContext?.User.Identity?.Name ?? "Unknown";

                var note = new IncidentValidationNotes
                {
                    IncidentId = incident.Id,
                    IncidentValidationId = incidentValidation.Id,
                    Notes = request.ValidationNotes,
                };

                await _db.IncidentValidationNotes.AddAsync(note);

                #region IncidentValidationRestoration
                // 9. Create default Restoration tasks
                await SeedDefaultRestorationTasks(incidentValidation.Id, request.Id);
                #endregion

                #region IncidentValidationCloseout
                // 10. Create default Closeout tasks
                await SeedDefaultCloseoutTasks(incidentValidation.Id, request.Id);
                #endregion

                #region  Save everything in one go
                // 6. Save everything in one go
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return incidentValidation.Id;
                #endregion
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveIncidentValidation.");
                return 0;
            }
        }
        public async Task<long> SaveIncidentValidation1(IncidentSubmitViewModel request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                #region IncidentValidation
                // 1. Save main IncidentValidation
                var incidentValidation = new IncidentValidation
                {
                    IncidentId = request.Id,
                    IsMarkFalseAlarm = false,
                    ValidationNotes = request.ValidationNotes ?? "test",
                    AssignResponseTeams = "1",//request.AssignResponseTeams,
                    ConfirmedSeverityLevelId = request.ConfirmedSeverityLevelId,
                    DiscoveryPerimeterId = request.DiscoveryPerimeterId,
                };

                await _db.IncidentValidations.AddAsync(incidentValidation);
                await _db.SaveChangesAsync();

                //var insertedValidationId = incidentValidation.Id;
                #endregion

                #region Policies
                // 2. Policies
                var policies = request.listSubmitPolicyVM.Select(item => new IncidentValidationPolicy
                {
                    IncidentId = request.Id,
                    IncidentValidationId = incidentValidation.Id,
                    PolicyId = item.PolicyId,
                    Status = item.Status,
                    TeamIds = item.Teams != null && item.Teams.Any()
                                ? string.Join(",", item.Teams)
                                : string.Empty
                }).ToList();

                if (policies.Any())
                    await _db.IncidentValidationPolicies.AddRangeAsync(policies);
                #endregion

                #region  Communication history
                // 3. Communication history
                var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "Communication");
                if (!Directory.Exists(uploadRoot))
                    Directory.CreateDirectory(uploadRoot);

                var communications = request.listSubmitCommunicationVM.Select(item =>
                {
                    var fileList = MoveFilesToPermanentStorage(item.FileMeta, uploadRoot);

                    return new IncidentValidationCommunicationHistory
                    {
                        IncidentId = request.Id,
                        IncidentValidationId = incidentValidation.Id,
                        ImageUrl = string.Join(",", fileList),
                        MessageType = item.MessageType,
                        Message = item.Message,
                        RecipientsIds = item.RecipientsIds ?? "1",
                        TimeStamp = item.TimeStamp,
                        UserName = item.UserName,
                    };
                }).ToList();

                if (communications.Any())
                    await _db.IncidentValidationCommunicationHistories.AddRangeAsync(communications);
                #endregion

                #region Incident Validation Locations
                // 4. Add Incident Validation Locations
                if (request.listSubmitValidationLocationVM.Any())
                {
                    var validationLocation = request.listSubmitValidationLocationVM.Select(item =>
                    {
                        return new IncidentValidationLocation
                        {
                            IncidentId = request.Id,
                            IncidentValidationId = incidentValidation.Id,
                            ICPLocation = item.ICPLocation ?? string.Empty,
                            DiscoveryPerimeterId = item.DiscoveryPerimeter ?? 0,
                            Lat = item.Lat,
                            Lng = item.Lon,
                            Source = item.Source,
                            ConfirmedSeverityLevelId = item.SeverityID ?? 0,
                            AdditionalLocationId = item.LocationId
                        };
                    }).ToList();
                    await _db.IncidentValidationLocations.AddRangeAsync(validationLocation);
                }

                // 6. Save main IncidentValidationAssignedRole
                var IncidentValidationAssignedRole = new IncidentValidationAssignedRole
                {
                    IncidentValidationId = incidentValidation.Id,
                    IncidentId = request.Id,
                    IncidentCommander = request.assignedRole.IncidentCommanderId,
                    FieldEnvRep = request.assignedRole.FieldEnvRepId,
                    GEC_Coordinator = request.assignedRole.GECCoordinatorId,
                    EngineeringLead = request.assignedRole.EngineeringLeadId,
                    ActiveStatus = ActiveStatus.Active
                };
                await _db.IncidentValidationAssignedRoles.AddAsync(IncidentValidationAssignedRole);

                // 7. Save main IncidentValidationAssignedRole
                var IncidentValidationGate = new IncidentValidationGate
                {
                    IncidentValidationId = incidentValidation.Id,
                    IncidentId = request.Id,
                    ContainmentAcknowledgement = request.validationGates.ContainmentAcknowledgement,
                    Exception = request.validationGates.Exception,
                    IndependentInspection = request.validationGates.IndependentInspection,
                    Regulatory = request.validationGates.Regulatory,
                    IsOtherEvent = request.validationGates.IsOtherEvent,
                    OtherEventDetail = request.validationGates.OtherEventDetail,
                    ActiveStatus = ActiveStatus.Active
                };
                await _db.IncidentValidationGates.AddAsync(IncidentValidationGate);

                #endregion

                #region Incident Validation Personel Info 
                // 4. Add Incident Validation Personel Info 
                if (request.listSubmitPersonalDataVM.Any())
                {
                    var validationPersonelInfo = request.listSubmitPersonalDataVM.Select(item =>
                    {
                        return new IncidentValidationPersonnel
                        {
                            IncidentId = request.Id,
                            IncidentValidationId = incidentValidation.Id,
                            CompanyId = item.CompanyId,
                            RoleId = item.RoleId,
                            ShiftId = item.ShiftId,
                            UserId = item.UserId
                        };
                    }).ToList();
                    await _db.IncidentValidationPersonnels.AddRangeAsync(validationPersonelInfo);
                }
                #endregion

                #region Incident Validation Assestment
                if (!string.IsNullOrWhiteSpace(request.incidentValidationAssessment))
                {
                    request.incidentSubmitValidationAssessment.IncidentId = request.Id;
                    request.incidentSubmitValidationAssessment.IncidentValidationId = incidentValidation.Id;
                    await _db.IncidentValidationAssessments.AddAsync(request.incidentSubmitValidationAssessment);
                }
                #endregion

                #region IncidentValidationRepair
                // 8. Save main IncidentValidationAssignedRole
                var IncidentValidationRepair = new IncidentValidationRepair
                {
                    IncidentValidationId = incidentValidation.Id,
                    IncidentId = request.Id,
                    SourceOfLeak = request.validationRepair.SourceOfLeak,
                    SourceOfLeakStatus = request.validationRepair.SourceOfLeakStatus,
                    PreventFurtherOutage = request.validationRepair.PreventFurtherOutage,
                    PreventFurtherOutageStatus = request.validationRepair.PreventFurtherOutageStatus,
                    VacuumTruckFitting = request.validationRepair.VacuumTruckFitting,
                    VacuumTruckFittingStatus = request.validationRepair.VacuumTruckFittingStatus,
                    ActiveStatus = ActiveStatus.Active
                };
                await _db.IncidentValidationRepairs.AddAsync(IncidentValidationRepair);
                #endregion

                #region IncidentValidationRestoration
                // 9. Create default Restoration tasks
                await SeedDefaultRestorationTasks(incidentValidation.Id, request.Id);
                #endregion

                #region IncidentValidationCloseout
                // 10. Create default Closeout tasks
                await SeedDefaultCloseoutTasks(incidentValidation.Id, request.Id);
                #endregion

                #region Incident Validation Task Info 
                // 4. Add Incident Validation Task Info 
                if (request.listSubmitTaskDataVM.Any())
                {
                    var validationTaskInfo = request.listSubmitTaskDataVM.Select(item =>
                    {
                        return new IncidentValidationTask
                        {
                            IncidentId = request.Id,
                            IncidentValidationId = incidentValidation.Id,
                            RoleIds = item.RoleIds ?? string.Empty,
                            StatusId = item.StatusId,
                            TaskDescription = item.TaskDescription ?? string.Empty
                        };
                    }).ToList();
                    await _db.IncidentValidationTasks.AddRangeAsync(validationTaskInfo);
                }
                #endregion

                #region Incident Validation Closeout Task Info 
                // 4. Add Incident Validation Closeout Task Info 
                if (request.listSubmitCloseoutTaskDataVM.Any())
                {
                    var validationCloseoutTaskInfo = request.listSubmitCloseoutTaskDataVM.Select(item =>
                    {
                        return new ValidationCloseout
                        {
                            IncidentId = request.Id,
                            IncidentValidationId = incidentValidation.Id,
                            Role = item.Role ?? string.Empty,
                            Status = item.Status,
                            Description = item.Description ?? string.Empty
                        };
                    }).ToList();
                    await _db.ValidationCloseouts.AddRangeAsync(validationCloseoutTaskInfo);
                }
                #endregion

                #region Update Incident record
                // 5. Update Incident record
                var incident = await _db.Incidents.FirstOrDefaultAsync(p => p.Id == request.Id);
                if (incident != null)
                {
                    var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userIdParsed = !string.IsNullOrEmpty(userId) ? long.Parse(userId) : 0;

                    // ⚠️ Replace with real logged-in user
                    var statusLegend = await _db.StatusLegends.FirstOrDefaultAsync(x => x.Name == StatusLegendEnum.Validated.ToString());

                    incident.StatusLegendId = statusLegend?.Id ?? (int)StatusLegendEnum.Validated;
                    //incident.SeverityLevelId = (int)SeverityEnum.Low;
                    incident.UpdatedOn = DateTime.Now;
                    incident.UpdatedBy = userIdParsed;
                }

                #endregion

                #region  Save everything in one go
                // 6. Save everything in one go
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return incidentValidation.Id;
                #endregion
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveIncidentValidation.");
                return 0;
            }
        }
        public async Task<List<TeamWithUsersViewModel>> GetIncidentTeamUsers()
        {
            try
            {
                var usersTeams = await _db.IncidentUsers
                    .Include(p => p.Team)
                    .Where(p => !p.IsDeleted && p.TeamId != null)
                    .ToListAsync();

                var teamWiseUsers = usersTeams
                    .GroupBy(u => new { u.TeamId, u.Team.Name })
                    .Select(g => new TeamWithUsersViewModel
                    {
                        TeamId = g.Key.TeamId,
                        TeamName = g.Key.Name,
                        Users = g.Select(u => new IncidentUser
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Telephone = u.Telephone ?? string.Empty,
                            Email = u.Email ?? string.Empty
                        }).ToList()
                    })
                    .OrderBy(t => t.TeamName)
                    .ToList();

                return teamWiseUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetIncidentTeamUsers.");
                return new List<TeamWithUsersViewModel>();
            }
        }

        public async Task<List<IncidentAdditionalLocationViewModel>> GetIncidentAdditionalLocationByIncident(long incidentId)
        {
            try
            {
                // Check if primary additional location exists
                bool primaryExists = await _db.AdditionalLocations
                    .AnyAsync(p => !p.IsDeleted && p.IncidentID == incidentId && p.IsPrimaryLocation);

                // If not, create from main incident info
                if (!primaryExists)
                {
                    var incidentInfo = await _db.Incidents
                        .Where(p => !p.IsDeleted && p.Id == incidentId)
                        .Select(p => new
                        {
                            p.Id,
                            p.LocationAddress,
                            p.Lat,
                            p.Lng,
                            p.Landmark,
                            p.ServiceAccount,
                            p.AssetIds
                        })
                        .FirstOrDefaultAsync();

                    if (incidentInfo != null)
                    {
                        var additionalLocation = new AdditionalLocationViewModel
                        {
                            LocationAddress = incidentInfo.LocationAddress ?? string.Empty,
                            Latitude = incidentInfo.Lat,
                            Longitude = incidentInfo.Lng,
                            IncidentId = incidentInfo.Id,
                            NearestIntersection = incidentInfo.Landmark,
                            ServiceAccount = incidentInfo.ServiceAccount,
                            PerimeterType = false,
                            PerimeterTypeDigit = 0,
                            AssetIDs = incidentInfo.AssetIds ?? string.Empty,
                            IsPrimaryLocation = true
                        };

                        await _iAdditionalLocationsService.SaveadditionalLocations(
                            new List<AdditionalLocationViewModel> { additionalLocation });
                    }
                }

                // Fetch all locations (primary first)
                var additionalLocations = await _db.AdditionalLocations
                    .Where(p => !p.IsDeleted && p.IncidentID == incidentId)
                    .OrderByDescending(p => p.IsPrimaryLocation)
                    .Select(a => new IncidentAdditionalLocationViewModel
                    {
                        Id = a.Id,
                        IncidentId = a.IncidentID,
                        AdditionalLocation = a.LocationAddress ?? string.Empty,
                        Lat = a.Latitude,
                        Long = a.Longitude,
                        IsPrimaryLocation = a.IsPrimaryLocation
                    })
                    .ToListAsync();

                return additionalLocations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in GetIncidentAdditionalLocationByIncident for IncidentId: {IncidentId}", incidentId);
                return new List<IncidentAdditionalLocationViewModel>();
            }
        }

        public async Task<GeocodeResult?> GetLatLngFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return new GeocodeResult { Lat = 0, Lng = 0 };

            try
            {
                using var client = new HttpClient();
                string url = $"https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates" +
                             $"?f=json&SingleLine={Uri.EscapeDataString(address)}";

                var response = await client.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                if (!doc.RootElement.TryGetProperty("candidates", out var candidates))
                    return new GeocodeResult { Lat = 0, Lng = 0 };

                if (candidates.GetArrayLength() > 0)
                {
                    var location = candidates[0].GetProperty("location");
                    return new GeocodeResult
                    {
                        Lat = location.GetProperty("y").GetDouble(),
                        Lng = location.GetProperty("x").GetDouble()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetLatLngFromAddress.");
                return new GeocodeResult { Lat = 0, Lng = 0 };
            }

            return new GeocodeResult { Lat = 0, Lng = 0 };
        }

        // ---------- new: save additional location ----------
        public async Task<long> AddAdditionalLocationAsync(long incidentId, string address, double lat, double lng)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // If you have an AdditionalLocation entity defined (Models.AdditionalLocation)
                var additional = new AdditionalLocations
                {
                    IncidentID = incidentId,
                    LocationAddress = address,
                    Latitude = lat,
                    Longitude = lng,
                    CreatedOn = DateTime.Now,
                    CreatedBy = GetCurrentUserIdOrDefault(),
                    IsDeleted = false,
                    //ActiveStatus = 1
                };

                await _db.AdditionalLocations.AddAsync(additional);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return additional.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error AddAdditionalLocationAsync.");
                return 0;
            }
        }

        public async Task<long> DeleteAdditionalLocationAsync(long id)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {

                var addLocation = await _db.AdditionalLocations.Where(p => p.Id == id).FirstOrDefaultAsync();
                if (addLocation != null)
                {
                    addLocation.IsDeleted = true;
                }
                else
                {
                    return 0;
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return addLocation.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SavePolicy.");
                return 0;
            }
        }


        #region private event
        /// <summary>
        /// Moves files from temp folder to permanent storage.
        /// Returns list of relative paths.
        /// </summary>
        private static List<string> MoveFilesToPermanentStorage(IEnumerable<FileMeta> fileMeta, string uploadRoot)
        {
            var fileList = new List<string>();

            foreach (var file in fileMeta)
            {
                if (string.IsNullOrWhiteSpace(file.TempPath))
                    continue;

                var destinationPath = Path.Combine(uploadRoot, file.FileName);
                var relativePath = $"/Storage/uploads/Communication/{file.FileName}";

                if (!File.Exists(destinationPath))
                {
                    File.Move(file.TempPath, destinationPath);
                }

                fileList.Add(relativePath);
            }

            return fileList;
        }
        private async Task<string> GetEventTypes(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return string.Empty;

            var idArray = ids.Split(",", StringSplitOptions.RemoveEmptyEntries)
                             .Select(id => long.TryParse(id.Trim(), out var val) ? val : (long?)null)
                             .Where(val => val.HasValue)
                             .Select(val => val.Value)
                             .ToList();

            var eventTypes = await _db.EventTypes
                                      .Where(a => idArray.Contains(a.Id))
                                      .Select(a => a.Name)
                                      .ToListAsync();

            return string.Join(",", eventTypes);
        }
        private string GetDate(string callTime)
        {
            if (TryParseCallTime(callTime, out var dt))
            {
                return dt.ToString("MMM dd, yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }
        private bool TryParseCallTime(string callTime, out DateTime dateTime)
        {
            return DateTime.TryParse(callTime, out dateTime);
        }
        private async Task<List<string>> GetAssets(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return new List<string>();

            var idArray = ids.Split(",", StringSplitOptions.RemoveEmptyEntries)
                             .Select(id => long.TryParse(id.Trim(), out var val) ? val : (long?)null)
                             .Where(val => val.HasValue)
                             .Select(val => val.Value)
                             .ToList();

            var assetNames = await _db.AssetIncidents
                                      .Where(a => idArray.Contains(a.Id))
                                      .Select(a => a.Name)
                                      .ToListAsync();

            return assetNames;
        }
        private string GetIndicator(long? value) =>
           value switch
           {
               1 => "Yes",
               0 => "No",
               2 => "N/A",
               _ => string.Empty
           };
        private long GetCurrentUserIdOrDefault()
        {
            try
            {
                var userId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                return !string.IsNullOrEmpty(userId) && long.TryParse(userId, out var parsed) ? parsed : 1L;
            }
            catch
            {
                return 1L;
            }
        }

        private async Task SeedDefaultRestorationTasks(long incidentValidationId, long incidentId)
        {
            try
            {
                // Get default status (Pending)
                var defaultStatus = await _db.Progress
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Name.ToLower().Contains("Not Started"));
                
                var defaultStatusId = defaultStatus?.Id ?? 1; // Fallback to ID 1 if not found

                // Get role IDs by name
                var engineeringRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("Engineering"));
                var mrcRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && (r.Name.ToLower().Contains("M&R Crew") || r.Name.ToLower().Contains("maintenance")));
                var customerServiceRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("Customer Service"));
                var icRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("IC"));
                var ferRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("FER"));

                // Define the 5 default Restoration tasks
                var defaultRestorationTasks = new List<IncidentValidationTask>
                {
                    new IncidentValidationTask
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        TaskDescription = "Pressure test completed for Segment A",
                        RoleIds = engineeringRole?.Id.ToString() ?? "1", // Engineering
                        StatusId = defaultStatusId,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new IncidentValidationTask
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        TaskDescription = "Pressure test in progress for Segment B",
                        RoleIds = engineeringRole?.Id.ToString() ?? "1", // Engineering
                        StatusId = defaultStatusId,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new IncidentValidationTask
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        TaskDescription = "Inspect and reinstall regulators and meters",
                        RoleIds = mrcRole?.Id.ToString() ?? "2", // M&R Crew
                        StatusId = defaultStatusId,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new IncidentValidationTask
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        TaskDescription = "Relight customer appliances (residential)",
                        RoleIds = customerServiceRole?.Id.ToString() ?? "3", // Customer Service
                        StatusId = defaultStatusId,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new IncidentValidationTask
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        TaskDescription = "Confirm all lines dried and restored",
                        RoleIds = $"{icRole?.Id ?? 1},{ferRole?.Id ?? 2}", // IC / FER
                        StatusId = defaultStatusId,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    }
                };

                await _db.IncidentValidationTasks.AddRangeAsync(defaultRestorationTasks);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default Restoration tasks for Incident {IncidentId}", incidentId);
            }
        }

        private async Task SeedDefaultCloseoutTasks(long incidentValidationId, long incidentId)
        {
            try
            {
                // Get default status (Not Started)
                var defaultStatus = await _db.Progress
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Name.ToLower().Contains("Not Started"));
                
                var defaultStatusId = defaultStatus?.Id ?? 1; // Fallback to ID 1 if not found

                // Get role names for closeout tasks
                var icRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("IC"));
                var gecRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("GEC"));
                var engineeringRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("Engineering"));
                var ferRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("FER"));
                var vendorRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("Vendor"));
                var complianceRole = await _db.IncidentRoles
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("Compliance"));
                var accountingRole = await _db.IncidentRoles
                   .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name.ToLower().Contains("Accounting"));

                // Define the 5 default Closeout tasks
                var defaultCloseoutTasks = new List<ValidationCloseout>
                {
                    new ValidationCloseout
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        Description = "Generate Major Customer Report (MCR)",
                        Role = $"{icRole?.Id.ToString() ?? "IC"},{gecRole?.Id.ToString() ?? "1"}", // IC / GEC
                        Status = defaultStatusId.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new ValidationCloseout
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        Description = "Prepare CPUC / PHMSA Report",
                        Role = $"{engineeringRole?.Id.ToString() ?? "1"},{complianceRole?.Id.ToString() ?? "1"}", // Engineering / Compliance
                        Status = defaultStatusId.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new ValidationCloseout
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        Description = "Environmental Summary & Disposal Documentation",
                        Role = $"{ferRole?.Id.ToString() ?? "1"},{vendorRole?.Id.ToString() ?? "1"}", // FER / Vendor
                        Status = defaultStatusId.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new ValidationCloseout
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        Description = "Finalize Cost Breakdown (Labor, Equipment, Material)",
                        Role = $"{gecRole?.Id.ToString() ?? "1"},{accountingRole?.Id.ToString() ??"1"}", 
                        Status = defaultStatusId.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    },
                    new ValidationCloseout
                    {
                        IncidentId = incidentId,
                        IncidentValidationId = incidentValidationId,
                        Description = "Submit Lessons Learned Report",
                        Role = $"{icRole?.Id.ToString() ?? "1"},{ferRole?.Id.ToString()  ?? "1"},{engineeringRole?.Id.ToString()  ?? "1"}", // IC / FER / Engineering
                        Status = defaultStatusId.ToString(),
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false,
                        ActiveStatus = ActiveStatus.Active
                    }
                };

                await _db.ValidationCloseouts.AddRangeAsync(defaultCloseoutTasks);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default Closeout tasks for Incident {IncidentId}", incidentId);
            }
        }
        #endregion
    }
}
