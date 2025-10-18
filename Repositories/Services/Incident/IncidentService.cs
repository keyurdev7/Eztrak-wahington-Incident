using AutoMapper;

using Azure;

using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

using Enums;

using Helpers.Extensions;
using Helpers.File;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Services.ArcGis;
using Repositories.Services.ArcGis.Interface;
using Repositories.Shared.UserInfoServices.Interface;

using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.Dashboard;
using ViewModels.Incident;
using ViewModels.Shared;

using static ViewModels.Incident.IncidentViewModel;

namespace Repositories.Common
{
    public class IncidentService : IIncidentService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IncidentService> _logger;
        private readonly IAdditionalLocationsService _iAdditionalLocationsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IncidentService(ApplicationDbContext db, ILogger<IncidentService> logger, IHttpContextAccessor httpContextAccessor, IAdditionalLocationsService iAdditionalLocationsService)
        {
            _db = db;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _iAdditionalLocationsService = iAdditionalLocationsService;
        }

        public async Task<IncidentViewModel> GetIncidentDropDown()
        {
            try
            {
                IncidentViewModel incidentViewModel = new();

                //var statusLegends = await _db.StatusLegends
                //    .Where(it => !it.IsDeleted)
                //    .OrderBy(it => it.Name)
                //    .Select(it => new SelectListItem
                //    {
                //        Value = it.Id.ToString(),
                //        Text = it.Name,
                //        Group = new SelectListGroup()
                //        {
                //            Name = it.Color
                //        }
                //    })
                //    .ToListAsync();

                // Define the fixed order mapping
                var orderMap = new Dictionary<string, int>
                    {
                        { "Submitted", 1 },
                        { "Validated", 2 },
                        { "Completed", 3 },
                        { "Cancelled", 4 }
                    };

                var statusLegends = await _db.StatusLegends
                    .Where(it => !it.IsDeleted)
                    .ToListAsync();

                // Apply the fixed order
                var statusLegendsList = statusLegends
                    .OrderBy(it => orderMap.ContainsKey(it.Name) ? orderMap[it.Name] : int.MaxValue)
                    .Select(it => new SelectListItem
                    {
                        Value = it.Id.ToString(),
                        Text = it.Name,
                        Group = new SelectListGroup
                        {
                            Name = it.Color
                        }
                    })
                    .ToList();



                var severityLevels = await _db.SeverityLevels
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

                var relationships = await _db.Relationships
                   .Where(it => !it.IsDeleted)
                   .OrderBy(it => it.Name)
                   .Select(it => new SelectListItem
                   {
                       Value = it.Id.ToString(),
                       Text = it.Name
                   })
                   .ToListAsync();

                var assetIncidents = await _db.AssetIncidents
                  .Where(it => !it.IsDeleted)
                  .OrderBy(it => it.Name)
                  .Select(it => new SelectListItem
                  {
                      Value = it.Id.ToString(),
                      Text = it.Name
                  })
                  .ToListAsync();

                var eventTypes = await _db.EventTypes
                                .Where(it => !it.IsDeleted)
                                .OrderByDescending(it =>
                                    !string.IsNullOrWhiteSpace(it.Name) &&
                                    it.Name.Trim().ToLower() == "water intrusion")
                                .ThenBy(it => it.Name)
                                .Select(it => new SelectListItem
                                {
                                    Value = it.Id.ToString(),
                                    Text = !string.IsNullOrWhiteSpace(it.Description)
                                        ? it.Name + " (" + it.Description + ")"
                                        : it.Name
                                })
                                .ToListAsync();


                //var progress = await _db.
                //   .Where(it => !it.IsDeleted)
                //   .OrderBy(it => it.Name)
                //   .Select(it => new SelectListItem
                //   {
                //       Value = it.Id.ToString(),
                //       Text = it.Name
                //   })
                //   .ToListAsync();

                incidentViewModel.severityLevels = severityLevels;
                incidentViewModel.statusLegends = statusLegendsList;
                incidentViewModel.incidentCellerInformation.Relationships = relationships;
                incidentViewModel.incidentiLocation.AssetsIncidentList = assetIncidents;
                incidentViewModel.incidentDetails.EventTypes = eventTypes;

                return incidentViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentDropDown.");
                return new IncidentViewModel()!;
            }
        }

        public async Task<string> SaveIncident(IncidentViewModel viewModel)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var latLong = await GetLatLngFromAddress(viewModel.incidentiLocation.Address);

                // Generate IncidentID once
                var totalIncidentCount = await _db.Incidents.IgnoreQueryFilters().CountAsync();
                var incidentId = $"INC-{(totalIncidentCount + 1):D4}";

                // Save file if available
                var imageUrl = viewModel.incidentSupportingInfoViewModel?.File != null && viewModel.incidentSupportingInfoViewModel?.File.Count > 0
                    ? await SaveAttachments(viewModel.incidentSupportingInfoViewModel.File)
                    : null;

                if (viewModel.incidentSupportingInfoViewModel != null)
                    viewModel.incidentSupportingInfoViewModel.ImageUrl = imageUrl;

                // Map ViewModel → Entity
                var incident = new Incident
                {
                    IncidentID = incidentId,
                    StatusLegendId = (int)StatusLegendEnum.Submitted,
                    SeverityLevelId = viewModel.severityLevelId,
                    DescriptionIssue = viewModel.DescriptionIssue,

                    CallerAddress = viewModel.incidentCellerInformation?.CallerAddress,
                    CallerPhoneNumber = viewModel.incidentCellerInformation?.CallerPhoneNumber,
                    CallerName = viewModel.incidentCellerInformation?.CallerName,
                    CallTime = viewModel.incidentCellerInformation?.CallTime ?? DateTime.Now,
                    RelationshipId = viewModel.incidentCellerInformation?.RelationshipId,

                    EventTypeIds = viewModel.incidentDetails?.EventTypeIds,
                    IsOtherEvent = viewModel.incidentDetails.IsOtherEvent,
                    OtherEventDetail = viewModel.incidentDetails?.OtherEventDetail,

                    EvacuationRequiredId = viewModel.incidentEnvironmentalViewModel?.EvacuationRequiredID,
                    HissingPresentId = viewModel.incidentEnvironmentalViewModel?.HissingSoundPresentID,
                    VisibleDamagePresentId = viewModel.incidentEnvironmentalViewModel?.VisibleDamageID,
                    PeopleInjuredId = viewModel.incidentEnvironmentalViewModel?.PeopleInjuredID,
                    GasPresentId = viewModel.incidentEnvironmentalViewModel?.GasodorpresentID,
                    WaterPresentId = viewModel.incidentEnvironmentalViewModel?.WaterPresentID,
                    EmergencyResponseNotifiedId = viewModel.incidentEnvironmentalViewModel?.EmergencyResponseNotifiedID,

                    Landmark = viewModel.incidentiLocation?.Landmark,
                    LocationAddress = viewModel.incidentiLocation?.Address,
                    ServiceAccount = viewModel.incidentiLocation?.ServiceAccount,
                    AssetIds = viewModel.incidentiLocation?.AssetIDs,
                    IsSameCallerAddress = viewModel.incidentiLocation.IsSameCallerAddress,

                    ImageUrl = viewModel.incidentSupportingInfoViewModel?.ImageUrl,
                    SupportInfoNotes = viewModel.incidentSupportingInfoViewModel?.Notes,

                    Lat = latLong?.Lat ?? 0,
                    Lng = latLong?.Lng ?? 0,
                };

                // Save
                await _db.Incidents.AddAsync(incident);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                if (viewModel.additionalLocations != null && viewModel.additionalLocations.Count > 0 && !string.IsNullOrEmpty(incidentId))
                {
                    // Add primary location first
                    var primaryLocation = new AdditionalLocationViewModel
                    {
                        IncidentId = incident.Id,
                        Latitude = latLong?.Lat ?? 0,
                        Longitude = latLong?.Lng ?? 0,
                        IsPrimaryLocation = true,
                        LocationAddress = viewModel.incidentiLocation?.Address ?? string.Empty,
                        AssetIDs = viewModel.incidentiLocation?.AssetIDs ?? string.Empty,
                        NearestIntersection = viewModel.incidentiLocation?.Landmark ?? string.Empty,
                        ServiceAccount = viewModel.incidentiLocation?.ServiceAccount ?? string.Empty
                    };

                    // Make all existing additional locations non-primary and link incident id
                    viewModel.additionalLocations.ForEach(l =>
                    {
                        l.IncidentId = incident.Id;
                        l.IsPrimaryLocation = false;
                    });

                    // Add the primary location at the end
                    viewModel.additionalLocations.Add(primaryLocation);

                    // Save only if we have a valid incident id
                    if (incident.Id > 0)
                        await _iAdditionalLocationsService.SaveadditionalLocations(viewModel.additionalLocations);
                }

                return incidentId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SaveIncident.");
                return string.Empty;
            }
        }

        public async Task<string> UpdateIncident(IncidentViewModel viewModel)
        {
            try
            {
                var incident = await _db.Incidents.FirstOrDefaultAsync(p => p.Id == viewModel.Id);

                // If no incident, save as new
                if (incident == null)
                {
                    return await SaveIncident(viewModel);
                }

                var latLong = await GetLatLngFromAddress(viewModel.incidentiLocation.Address);

                // Save file if available
                var file = viewModel.incidentSupportingInfoViewModel?.File;
                var imageUrl = (file?.Count > 0) ? await SaveAttachments(file) : null;

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    viewModel.incidentSupportingInfoViewModel!.ImageUrl = imageUrl;
                }
                else
                {
                    viewModel.incidentSupportingInfoViewModel!.ImageUrl ??= incident.ImageUrl;
                }

                // Update entity from ViewModel
                incident.SeverityLevelId = viewModel.severityLevelId;
                incident.DescriptionIssue = viewModel.DescriptionIssue;

                var caller = viewModel.incidentCellerInformation;
                incident.CallerAddress = caller?.CallerAddress;
                incident.CallerPhoneNumber = caller?.CallerPhoneNumber;
                incident.CallerName = caller?.CallerName;
                incident.CallTime = caller?.CallTime ?? incident.CallTime;
                incident.RelationshipId = caller?.RelationshipId;

                var details = viewModel.incidentDetails;
                incident.EventTypeIds = details?.EventTypeIds;
                incident.IsOtherEvent = details?.IsOtherEvent ?? false;
                incident.OtherEventDetail = details?.OtherEventDetail;

                var env = viewModel.incidentEnvironmentalViewModel;
                incident.EvacuationRequiredId = env?.EvacuationRequiredID;
                incident.HissingPresentId = env?.HissingSoundPresentID;
                incident.VisibleDamagePresentId = env?.VisibleDamageID;
                incident.PeopleInjuredId = env?.PeopleInjuredID;
                incident.GasPresentId = env?.GasodorpresentID;
                incident.WaterPresentId = env?.WaterPresentID;
                incident.EmergencyResponseNotifiedId = env?.EmergencyResponseNotifiedID;

                var loc = viewModel.incidentiLocation;
                incident.Landmark = loc?.Landmark;
                incident.LocationAddress = loc?.Address;
                incident.ServiceAccount = loc?.ServiceAccount;
                incident.AssetIds = loc?.AssetIDs;
                incident.IsSameCallerAddress = loc?.IsSameCallerAddress ?? false;

                var support = viewModel.incidentSupportingInfoViewModel;
                incident.ImageUrl = support?.ImageUrl ?? incident.ImageUrl;
                incident.SupportInfoNotes = support?.Notes;

                incident.Lat = latLong?.Lat ?? 0;
                incident.Lng = latLong?.Lng ?? 0;

                // Save within transaction
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return incident.IncidentID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating incident.");
                return string.Empty;
            }
        }

        public async Task<List<IncidentGridViewModel>> GetIncidentList(FilterRequest request)
        {

            List<IncidentGridViewModel> incidentGridViews = new();
            try
            {
                var query = _db.Incidents
                             .Include(p => p.StatusLegend)
                             .Include(p => p.Relationship)
                             //.Include(p => p.EventType)
                             .Include(p => p.SeverityLevel)
                             .AsQueryable();


                if (request != null)
                {
                    if (request.severityId > 0)
                    {
                        query = query.Where(p => p.SeverityLevelId == request.severityId);
                    }

                    if (request.statusId > 0)
                    {
                        query = query.Where(p => p.StatusLegendId == request.statusId);
                    }

                    if (!string.IsNullOrWhiteSpace(request.description))
                    {
                        query = query.Where(p => p.DescriptionIssue.Contains(request.description));
                    }
                }

                query = query.OrderByDescending(p => p.CallTime);

                //// --- Order by "Submitted" status on top ---
                //query = query.OrderByDescending(p => p.StatusLegend.Name == StatusLegendEnum.Submitted.ToString());

                var incidentsList = await query.ToListAsync();
                var incidentIds = incidentsList.Select(i => i.Id).ToList();

                var additionalCounts = new List<(long IncidentId, int Count)>();
                if (incidentIds.Any())
                {
                    additionalCounts = await _db.AdditionalLocations
                        .Where(al => !al.IsDeleted && al.IncidentID.HasValue && incidentIds.Contains(al.IncidentID.Value))
                        .GroupBy(al => al.IncidentID)
                        .Select(g => new { IncidentId = g.Key.Value, Count = g.Count() })
                        .ToListAsync()
                        .ContinueWith(t => t.Result.Select(x => (x.IncidentId, x.Count)).ToList());
                }

                foreach (var item in incidentsList)
                {
                    var addCount = additionalCounts.FirstOrDefault(a => a.IncidentId == item.Id).Count;

                    incidentGridViews.Add(new IncidentGridViewModel()
                    {
                        CallDate = GetDate(Convert.ToString(item.CallTime)),
                        CallTime = GetTime(Convert.ToString(item.CallTime)),
                        AssetId = await GetAssets(item.AssetIds ?? string.Empty),
                        DescriptionIssue = item.DescriptionIssue ?? string.Empty,
                        EventTypeId = await GetEventTypes(item.EventTypeIds ?? string.Empty),
                        GasESIndicator = GetIndicator(item.GasPresentId),
                        Id = item.Id,
                        Intersection = item.Landmark ?? string.Empty,
                        LocationAddress = item.LocationAddress ?? string.Empty,
                        Severity = item.SeverityLevel?.Name ?? string.Empty,
                        SeverityId = item?.SeverityLevelId,
                        StatusLegend = item.StatusLegend?.Name ?? string.Empty,
                        StatusLegendColor = item.StatusLegend?.Color ?? string.Empty,
                        StatusLegendId = item.StatusLegendId,
                        RelationShipName = item.Relationship?.Name ?? string.Empty,
                        RelationShipId = item?.RelationshipId,
                        AdditionalLocationCount = addCount
                    });
                }
                return incidentGridViews;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentList.");
                return new List<IncidentGridViewModel>();
            }
        }

        //public async Task<string?> ChangeIncidentStatus(long incidentId, string statusText)
        //{
        //    await using var transaction = await _db.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var incident = await _db.Incidents.FirstOrDefaultAsync(p => p.Id == incidentId);

        //        if (incident == null)
        //        {
        //            await transaction.RollbackAsync();
        //            return null; // or string.Empty if you want
        //        }

        //        if (Enum.TryParse<StatusLegendEnum>(statusText, true, out var status))
        //        {
        //            incident.StatusLegendId = (long)status;

        //            await _db.SaveChangesAsync();
        //            await transaction.CommitAsync();
        //        }
        //        else
        //        {
        //            await transaction.RollbackAsync();
        //            return null;
        //        }

        //        return incident.IncidentID;
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error ChangeIncidentStatus.");
        //        return null; // or string.Empty
        //    }
        //}
        public async Task<string?> ChangeIncidentStatus(long incidentId, string statusText)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var incident = await _db.Incidents.FirstOrDefaultAsync(p => p.Id == incidentId);

                if (incident == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                // find matching status from StatusLegends table
                var statusLegend = await _db.StatusLegends
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == statusText.ToLower());

                if (statusLegend == null)
                {
                    await transaction.RollbackAsync();
                    return null; // no such status in DB
                }

                var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdParsed = !string.IsNullOrEmpty(userId) ? long.Parse(userId) : 0;

                // update incident status
                incident.StatusLegendId = statusLegend.Id;
                incident.UpdatedOn = DateTime.UtcNow;

                // add history entry
                var history = new IncidentHistory
                {
                    IncidentId = incident.Id,
                    StatusLegendId = statusLegend.Id,
                    Description = $"Status changed to {statusLegend.Name}",
                    IsDeleted = false,
                    ActiveStatus = Enums.ActiveStatus.Active,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userIdParsed,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = userIdParsed
                };

                _db.IncidentHistories.Add(history);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return incident.IncidentID;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error ChangeIncidentStatus.");
                return null;
            }
        }

        public async Task<IncidentViewModel> GetById(long incidentId)
        {
            var incidentViewModel = new IncidentViewModel();

            try
            {
                incidentViewModel = await GetIncidentDropDown();

                var incident = await _db.Incidents.FirstOrDefaultAsync(p => p.Id == incidentId);

                if (incident == null)
                {
                    return new IncidentViewModel();
                }

                incidentViewModel.Id = incident?.Id;

                incidentViewModel.DescriptionIssue = incident?.DescriptionIssue;
                incidentViewModel.severityLevelId = incident?.SeverityLevelId;
                incidentViewModel.incidentiLocation.Address = incident?.LocationAddress;
                incidentViewModel.incidentiLocation.AssetIDs = incident?.AssetIds;
                incidentViewModel.incidentiLocation.Landmark = incident?.Landmark;
                incidentViewModel.incidentiLocation.ServiceAccount = incident?.ServiceAccount;
                incidentViewModel.incidentiLocation.IsSameCallerAddress = incident.IsSameCallerAddress;

                incidentViewModel.incidentDetails.EventTypeIds = incident?.EventTypeIds;
                incidentViewModel.incidentDetails.OtherEventDetail = incident?.OtherEventDetail;
                incidentViewModel.incidentDetails.IsOtherEvent = incident.IsOtherEvent;

                incidentViewModel.incidentCellerInformation.CallerPhoneNumber = incident.CallerPhoneNumber;
                incidentViewModel.incidentCellerInformation.CallerAddress = incident.CallerAddress;
                incidentViewModel.incidentCellerInformation.CallerName = incident.CallerName;
                incidentViewModel.incidentCellerInformation.CallTime = incident.CallTime;
                incidentViewModel.incidentCellerInformation.RelationshipId = incident.RelationshipId;

                incidentViewModel.incidentEnvironmentalViewModel.PeopleInjuredID = incident.PeopleInjuredId;
                incidentViewModel.incidentEnvironmentalViewModel.HissingSoundPresentID = incident.HissingPresentId;
                incidentViewModel.incidentEnvironmentalViewModel.EvacuationRequiredID = incident.EvacuationRequiredId;
                incidentViewModel.incidentEnvironmentalViewModel.VisibleDamageID = incident.VisibleDamagePresentId;
                incidentViewModel.incidentEnvironmentalViewModel.GasodorpresentID = incident.GasPresentId;
                incidentViewModel.incidentEnvironmentalViewModel.WaterPresentID = incident.WaterPresentId;
                incidentViewModel.incidentEnvironmentalViewModel.EmergencyResponseNotifiedID = incident.EmergencyResponseNotifiedId;

                incidentViewModel.incidentSupportingInfoViewModel.ImageUrl = incident.ImageUrl;
                incidentViewModel.incidentSupportingInfoViewModel.Notes = incident.SupportInfoNotes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById.");
                return new IncidentViewModel();
            }

            return incidentViewModel;
        }

        public async Task<IncidentViewModel> GetIncidentDetailsById(long id)
        {
            try
            {
                var incident = await _db.Incidents
                    .Include(i => i.StatusLegend)
                    .Include(i => i.SeverityLevel)
                    .Include(i => i.Relationship)
                    .FirstOrDefaultAsync(i => i.Id == id);

                var incidentValidation = (from i in _db.IncidentValidations
                                          join s in _db.SeverityLevels on i.ConfirmedSeverityLevelId equals s.Id
                                          where i.IncidentId == id
                                          select new IncidentValidationsDetailsViewModel
                                          {
                                              // IncidentValidation properties
                                              IncidentValidationId = i.Id,
                                              ConfirmedSeverityLevelId = i.ConfirmedSeverityLevelId,
                                              DiscoveryPerimeterId = i.DiscoveryPerimeterId,
                                              ValidationNotes = i.ValidationNotes,
                                              CreatedBy = i.UpdatedBy,
                                              CreatedOn = i.UpdatedOn,

                                              SeverityLevelName = s.Name,
                                              SeverityLevelColor = s.Color,

                                              IncidentValidationCommunicationHistoriesViewModelList = _db.IncidentValidationCommunicationHistories
                                                   .Where(ih => ih.IncidentId == i.IncidentId)
                                                   .OrderByDescending(ih => ih.CreatedOn)
                                                   .Select(ih => new IncidentValidationCommunicationHistoriesViewModel
                                                   {
                                                       UserName = ih.UserName,
                                                       Message = ih.Message,
                                                       TimeStamp = ih.TimeStamp,
                                                       ReceipientsIds = ih.RecipientsIds,
                                                       ImageUrl = ih.ImageUrl,
                                                       MessageType = ih.MessageType
                                                   })
                                                   .ToList()
                                          }).ToList() ?? new List<IncidentValidationsDetailsViewModel>();

                var incidentPolicies = await _db.IncidentValidationPolicies
                    .Where(ivp => ivp.IncidentId == id)
                    .ToListAsync();

                var teams = await _db.IncidentTeams.ToListAsync();
                var policies = await _db.Policies.ToListAsync();
                var severityLevels = await _db.SeverityLevels.Where(p => !p.IsDeleted).ToListAsync();

                var workStepsData = incidentPolicies
                    .SelectMany(ivp => ivp.TeamIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(teamId => new { ivp, teamId = int.Parse(teamId) }))
                    .Join(teams,
                          x => x.teamId,
                          it => it.Id,
                          (x, it) => new { x.ivp, Team = it })
                    .Join(policies,
                          x => x.ivp.PolicyId,
                          p => p.Id,
                          (x, p) => new WorkStepViewModel
                          {
                              PolicyId = p.Id,
                              PolicyName = p.Name,
                              TeamId = x.Team.Id,
                              TeamName = x.Team.Name,
                              PolicySteps = p.PolicySteps ?? string.Empty
                          })
                    .GroupBy(ws => ws.PolicyName)
                    .Select(g => g.ToList())  // each group is a list
                    .ToList(); // list of lists

                #region IncidentValidationAssignedRoles
                var roles = await _db.IncidentValidationAssignedRoles
                                          .AsNoTracking()
                                          .Where(p => !p.IsDeleted && p.IncidentId == id)
                                          .ToListAsync();

                var userIds = roles
                            .SelectMany(r => new long?[] { r.EngineeringLead, r.FieldEnvRep, r.GEC_Coordinator, r.IncidentCommander })
                            .Where(x => x.HasValue)
                            .Select(x => x!.Value)
                            .Distinct()
                            .ToList();

                // only fetch referenced users (and ignore IsDeleted users)
                var users = userIds.Count == 0
                    ? new List<IncidentUser>() // replace IncidentUser with your user entity type
                    : await _db.IncidentUsers
                        .AsNoTracking()
                        .Where(u => !u.IsDeleted && userIds.Contains(u.Id))
                        .ToListAsync();

                // build dictionary for fast lookup
                var usersDict = users.ToDictionary(u => u.Id);

                // helper to get full name
                string GetFullName(long? userId)
                {
                    if (!userId.HasValue) return string.Empty;
                    return usersDict.TryGetValue(userId.Value, out var u)
                        ? $"{u.LastName} {u.FirstName}".Trim()
                        : string.Empty;
                }

                // project to your view models
                var IncidentValidationAssignedRoles = roles.Select(p => new IncidentValidationAssignedRolesViewModel
                {
                    Id = p.Id,
                    IncidentId = p.IncidentId,
                    IncidentValidationId = p.IncidentValidationId,

                    EngineeringLeadId = p.EngineeringLead,
                    EngineeringLeadName = GetFullName(p.EngineeringLead),

                    FieldEnvRepId = p.FieldEnvRep,
                    FieldEnvRepName = GetFullName(p.FieldEnvRep),

                    GEC_CoordinatorId = p.GEC_Coordinator,
                    GEC_CoordinatorName = GetFullName(p.GEC_Coordinator),

                    IncidentCommanderId = p.IncidentCommander,
                    IncidentCommanderName = GetFullName(p.IncidentCommander)
                }).ToList();
                #endregion

                #region IncidentValidationGates
                var validationGates = await _db.IncidentValidationGates
                                         .AsNoTracking()
                                         .Where(p => !p.IsDeleted && p.IncidentId == id)
                                         .ToListAsync();

                var validationGatesVM = validationGates.Select(p => new IncidentValidationGatesViewModel
                {
                    Id = p.Id,
                    IncidentId = p.IncidentId,
                    IncidentValidationId = p.IncidentValidationId,
                    ContainmentAcknowledgement = p.ContainmentAcknowledgement.Value ? "FER Signed" : "No",
                    Exception = p.Exception.Value ? "Yes" : "No",
                    IndependentInspection = p.IndependentInspection.Value ? "Yes" : "No",
                    Regulatory = GetRegulatory(p.Regulatory ?? string.Empty)

                }).ToList();
                #endregion

                #region IncidentAdditionalLocation
                //var validationAdditionalLocation = await _db.IncidentValidationLocations
                //                         .AsNoTracking()
                //                         .Where(p => !p.IsDeleted && p.IncidentId == id)
                //                         .FirstOrDefaultAsync();

                //var validationAdditionalLocationVM = validationAdditionalLocation.Select(p => new IncidentValidationLocationViewModel
                //{
                //    DiscoveryPerimeter = p.DiscoveryPerimeterId,
                //    ICPLocation = p.ICPLocation ?? string.Empty,
                //    LocationId = p.AdditionalLocationId,
                //    SeverityID = p.ConfirmedSeverityLevelId,
                //    Source = p.Source ?? string.Empty,
                //    SeverityName = severityLevels.Where(s => s.Id == p.ConfirmedSeverityLevelId).FirstOrDefault()?.Name
                //}).ToList();
                var validationAdditionalLocation = await _db.IncidentValidationLocations
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted && p.IncidentId == id)
                    .FirstOrDefaultAsync();

                IncidentValidationLocationViewModel validationAdditionalLocationVM = null;

                if (validationAdditionalLocation != null)
                {
                    validationAdditionalLocationVM = new IncidentValidationLocationViewModel
                    {
                        DiscoveryPerimeter = validationAdditionalLocation.DiscoveryPerimeterId,
                        ICPLocation = validationAdditionalLocation.ICPLocation ?? string.Empty,
                        LocationId = validationAdditionalLocation.AdditionalLocationId,
                        SeverityID = validationAdditionalLocation.ConfirmedSeverityLevelId,
                        Source = validationAdditionalLocation.Source ?? string.Empty,
                        SeverityName = severityLevels
                            .FirstOrDefault(s => s.Id == validationAdditionalLocation.ConfirmedSeverityLevelId)?.Name
                    };
                }
                #endregion

                #region Personnel
                var IncidentValidationPersonnels = (
                            from ivp in _db.IncidentValidationPersonnels
                            where ivp.IncidentId == id
                            join u in _db.IncidentUsers on ivp.UserId equals u.Id into userGroup
                            from u in userGroup.DefaultIfEmpty()

                            join c in _db.Company on ivp.CompanyId equals c.Id into companyGroup
                            from c in companyGroup.DefaultIfEmpty()

                            join r in _db.IncidentRoles on ivp.RoleId equals r.Id into roleGroup
                            from r in roleGroup.DefaultIfEmpty()

                            join s in _db.IncidentShifts on ivp.ShiftId equals s.Id into shiftGroup
                            from s in shiftGroup.DefaultIfEmpty()

                            join si in _db.IncidentUsers on ivp.SupervisorId equals si.Id into userGroup1
                            from si in userGroup1.DefaultIfEmpty()

                            select new IncidentValidationPersonnelsViewModel
                            {
                                IncidentValidationPersonnelsId = ivp.Id,
                                UserId = ivp.UserId,
                                CompanyId = ivp.CompanyId,
                                Name = (u.FirstName + " " + u.LastName).Trim(),
                                Company = c.Name,
                                Role = r.Name,
                                Type = u.EmployeeType,
                                Shift = s.Name,
                                TimeIn = ivp.TimeIn,
                                Supervisor = (si.FirstName + " " + si.LastName).Trim()
                            }
                        ).ToList() ?? new List<IncidentValidationPersonnelsViewModel>();
                var now = DateTime.Now;
                var IncidentId = id;
                var OnsiteNow = await _db.IncidentValidationPersonnels.CountAsync(p => p.IncidentId == id);
                var CheckedOutToday = _db.IncidentValidationPersonnels.Count(p => p.IncidentId == id && p.TimeIn != null && p.TimeIn.Value.Date == DateTime.UtcNow.Date);
                var personnelstime = await _db.IncidentValidationPersonnels.Where(p => !p.IsDeleted && p.IncidentId == id && p.TimeIn.HasValue && p.TimeIn.Value.Date == now.Date)
                                    .ToListAsync();
                var totalHoursToday = personnelstime.Sum(p => (now - p.TimeIn.Value).TotalHours);
                totalHoursToday = Math.Round(totalHoursToday, 2);

                var TotalDayShift = IncidentValidationPersonnels.Count(ds => ds.Shift == "Day Shift");
                var TotalNightShift = IncidentValidationPersonnels.Count(ds => ds.Shift == "Night Shift");
                var TotalEmployees = IncidentValidationPersonnels.Count(ds => ds.Type == "Employee");
                var TotalContractors = IncidentValidationPersonnels.Count(ds => ds.Type == "Contractor");
                var totalHoursByPerson = IncidentValidationPersonnels
                                        .Where(p => p.TimeIn.HasValue && p.TimeIn.Value.Date == now.Date)
                                        .GroupBy(p => p.Name) // or p.PersonnelName / p.FullName / p.EmployeeName (your property name)
                                        .Select(g => new IncidentValidationPersonnelsTopContributorsViewModel
                                        {
                                            Name = g.Key,
                                            TotalHoursToday = Math.Round(g.Sum(x => (now - x.TimeIn.Value).TotalHours), 2)
                                        })
                                        .OrderByDescending(x => x.TotalHoursToday)
                                        .ToList();
                double AvgHoursWorker = 0;
                if (CheckedOutToday > 0)
                {
                    AvgHoursWorker = Math.Round(totalHoursToday / CheckedOutToday, 2);
                }
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
                #endregion

                #region IncidentValidationNotes


                var IncidentvalidationNotes = await _db.IncidentValidationNotes
            .AsNoTracking()
            .Where(n => !n.IsDeleted && n.IncidentId == id)
            .OrderByDescending(n => n.CreatedOn)
             .Select(n => new IncidentValidationNoteViewModel
             {
                 Id = n.Id,
                 IncidentId = n.IncidentId,
                 IncidentValidationId = n.IncidentValidationId,
                 Notes = n.Notes,
                 CreatedBy = n.CreatedBy,
                 CreatedOn = n.CreatedOn
             })
                .ToListAsync();
                #endregion

                #region IncidentValidationRepair
                List<IncidentViewPostViewModel> ListPostDetailVM = new List<IncidentViewPostViewModel>();

                List<IncidentViewRepairListViewModel> ListvalidationRepairVM = await GetvalidationRepairVM(id);
                ListPostDetailVM = await GetPostDetailVM(id, 1);
                IncidentViewRepairViewModel validationRepairVM = new IncidentViewRepairViewModel();

                validationRepairVM.listIncidentViewRepairViewModel = ListvalidationRepairVM;
                validationRepairVM.listIncidentViewPostViewModel = ListPostDetailVM;
                #endregion

                #region IncidentValidationCloseout
                List<IncidentViewCloseoutListViewModel> ListvalidationCloseoutVM = await GetvalidationCloseoutVM(id);
                ListPostDetailVM = await GetPostDetailVM(id, 2);
                IncidentViewCloseoutViewModel validationCloseoutVM = new IncidentViewCloseoutViewModel();

                validationCloseoutVM.PersonnelInvolved = await _db.IncidentValidationPersonnels.CountAsync(p => p.IncidentId == id);
                validationCloseoutVM.TotalCloseOut = await _db.ValidationCloseouts.CountAsync(p => p.IncidentId == id);
                validationCloseoutVM.listIncidentViewCloseoutViewModel = ListvalidationCloseoutVM;
                validationCloseoutVM.listIncidentViewPostViewModel = ListPostDetailVM;


                IncidentViewRestorationListViewModel validationRestorationVM = new IncidentViewRestorationListViewModel();
                ListPostDetailVM = await GetPostDetailVM(id, 4);
                validationRestorationVM.listIncidentViewPostViewModel = ListPostDetailVM;

                #endregion
                #region IncidentValidationRestoration
                List<IncidentViewTaskListViewModel> ListvalidationTaskVM = await GetvalidationTaskVM(id);
                //List<IncidentViewPostViewModel> ListPostDetailVMTask = await GetPostDetailVM(id);

                IncidentViewTaskViewModel validationTaskVM = new IncidentViewTaskViewModel
                {
                    listIncidentViewTaskViewModel = ListvalidationTaskVM,
                    //  listIncidentViewPostViewModel = ListPostDetailVMTask
                };
                #endregion

                if (incident == null)
                    return new IncidentViewModel();

                var viewModel = new IncidentViewModel
                {
                    Id = incident.Id,
                    DescriptionIssue = incident.DescriptionIssue ?? string.Empty,
                    severityLevelId = incident.SeverityLevelId,
                    //severityLevelId = incident.StatusLegendId,


                    incidentDetails = new IncidentDetailsViewModel
                    {
                        EventTypeIds = incident.EventTypeIds ?? string.Empty,
                        IsOtherEvent = incident.IsOtherEvent,
                        OtherEventDetail = incident.OtherEventDetail ?? string.Empty,
                        EventTypes = new List<SelectListItem>()
                    },

                    incidentDetailByIdViewModel = new IncidentDetailByIdViewModel()
                    {
                        StatusLegendId = incident.StatusLegend.Id,
                        SeverityName = incident.SeverityLevel?.Name ?? string.Empty,
                        SeverityColor = incident.SeverityLevel?.Color ?? string.Empty,
                        StatusLegendName = incident.StatusLegend?.Name ?? string.Empty,
                        StatusLegendColor = incident.StatusLegend?.Color ?? string.Empty,
                        IncidentNumber = incident.IncidentID,
                        CreatedOn = incident.CreatedOn,
                        UpdatedOn = incident.UpdatedOn,
                        CreatedOnDate = GetDate(Convert.ToString(incident.CreatedOn)),
                        CreatedOnTime = GetTime(Convert.ToString(incident.CreatedOn)),
                    },

                    incidentCellerInformation = new IncidentCellerInformationViewModel
                    {
                        CallerName = incident.CallerName ?? string.Empty,
                        CallerPhoneNumber = incident.CallerPhoneNumber ?? string.Empty,
                        CallerAddress = incident.CallerAddress ?? string.Empty,
                        CallTime = incident.CallTime,
                        RelationshipId = incident.RelationshipId,
                        RelationshipName = incident.Relationship?.Name ?? string.Empty,
                        CallDateInFormat = GetDate(Convert.ToString(incident.CallTime)),
                        CallTimeInFormat = GetTime(Convert.ToString(incident.CallTime)),
                    },

                    incidentiLocation = new IncidentiLocationViewModel
                    {
                        Address = incident.LocationAddress ?? string.Empty,
                        Landmark = incident.Landmark ?? string.Empty,
                        ServiceAccount = incident.ServiceAccount,
                        AssetIDs = incident.AssetIds ?? string.Empty,
                        IsSameCallerAddress = incident.IsSameCallerAddress,
                        AssetsIncidentList = new List<SelectListItem>()
                    },

                    incidentEnvironmentalViewModel = new IncidentEnvironmentalViewModel
                    {
                        GasodorpresentID = incident.GasPresentId,
                        HissingSoundPresentID = incident.HissingPresentId,
                        VisibleDamageID = incident.VisibleDamagePresentId,
                        PeopleInjuredID = incident.PeopleInjuredId,
                        EvacuationRequiredID = incident.EvacuationRequiredId,
                        EmergencyResponseNotifiedID = incident.EmergencyResponseNotifiedId,
                        WaterPresentID = incident.WaterPresentId,

                        GasOdorText = GetIndicator(incident.GasPresentId),
                        WaterPresentText = GetIndicator(incident.WaterPresentId),
                        HissingSoundText = GetIndicator(incident.HissingPresentId),
                        VisibleDamageText = GetIndicator(incident.VisibleDamagePresentId),
                        PeopleInjuredText = GetIndicator(incident.PeopleInjuredId),
                        EvacuationRequiredText = GetIndicator(incident.EvacuationRequiredId),
                        EmergencyResponseNotifiedText = GetIndicator(incident.EmergencyResponseNotifiedId)
                    },

                    incidentSupportingInfoViewModel = new IncidentSupportingInfoViewModel
                    {
                        Notes = incident.SupportInfoNotes ?? string.Empty,
                        ImageUrl = incident.ImageUrl, // keep original value

                        // ✅ split comma-separated image URLs
                        ImageUrls = !string.IsNullOrEmpty(incident.ImageUrl)
                         ? incident.ImageUrl.Split(",", StringSplitOptions.RemoveEmptyEntries)
                           .Select(img => img.Trim())
                           .ToList()
                            : new List<string>()
                    },



                    incidentValidationsDetailsViewModel = new IncidentValidationsDetailsViewModel
                    {
                        IncidentValidationId = incidentValidation.FirstOrDefault() != null ? incidentValidation.FirstOrDefault().IncidentValidationId : 0,
                        ConfirmedSeverityLevelId = incidentValidation.FirstOrDefault() != null ? incidentValidation.FirstOrDefault().ConfirmedSeverityLevelId : 0,
                        DiscoveryPerimeterId = incidentValidation.FirstOrDefault() != null ? incidentValidation.FirstOrDefault().DiscoveryPerimeterId : 0,
                        DiscoveryPerimeterName = GetPerimeter(incidentValidation.FirstOrDefault()?.DiscoveryPerimeterId),
                        ValidationNotes = incidentValidation.FirstOrDefault()?.ValidationNotes,
                        CreatedBy = incidentValidation.FirstOrDefault() != null ? incidentValidation.FirstOrDefault().CreatedBy : 0,
                        CreatedDateInFormat = GetDate(Convert.ToString(incidentValidation.FirstOrDefault()?.CreatedOn)),
                        CreatedTimeInFormat = GetTime(Convert.ToString(incidentValidation.FirstOrDefault()?.CreatedOn)),
                        SeverityLevelName = incidentValidation.FirstOrDefault()?.SeverityLevelName,
                        SeverityLevelColor = incidentValidation.FirstOrDefault()?.SeverityLevelColor,
                        IncidentValidationCommunicationHistoriesViewModelList = incidentValidation.FirstOrDefault()?.IncidentValidationCommunicationHistoriesViewModelList,
                        IncidentValidationNotesList = IncidentvalidationNotes
                    },

                    workSteps = workStepsData.SelectMany(x => x).ToList(),

                    incidentValidationAssignedRolesViewModel = IncidentValidationAssignedRoles.FirstOrDefault() ?? new IncidentValidationAssignedRolesViewModel(),
                    incidentValidationGatesViewModel = validationGatesVM.FirstOrDefault() ?? new IncidentValidationGatesViewModel(),

                    //IncidentValidationLocations = validationAdditionalLocationVM ?? new List<IncidentValidationLocationViewModel>(),
                    IncidentValidationLocations = validationAdditionalLocationVM ?? new IncidentValidationLocationViewModel(),

                    #region Personnel
                    incidentValidationPersonnelsViewModel = IncidentValidationPersonnels,
                    incidentValidationPersonnelsTopContributorsViewModel = totalHoursByPerson,
                    incidentValidationPersonnelsCountViewModel = new IncidentValidationPersonnelsCountViewModel
                    {
                        OnsiteNowCount = OnsiteNow,
                        CheckedOutTodayCount = CheckedOutToday,
                        TotalHoursToday = totalHoursToday,
                        AvgHoursWorker = AvgHoursWorker,
                        TotalDayShift = TotalDayShift,
                        TotalNightShift = TotalNightShift,
                        TotalEmployees = TotalEmployees,
                        TotalContractors = TotalContractors,
                        IncidentId = IncidentId,
                    },
                    UserList = UserLisTTask,
                    CompanyList = companyList,
                    RoleList = rolesList,
                    ShiftsList = shiftsList,
                    severityLevels = severityLevelsTask
                    #endregion
                };

                // ✅ Set TeamsByPolicy for each WorkStepViewModel
                foreach (var policyGroup in workStepsData)
                {
                    var policyName = policyGroup.FirstOrDefault()?.PolicyName;
                    var teamNames = string.Join(" || ", policyGroup.Select(ws => ws.TeamName).Distinct());

                    foreach (var workStep in policyGroup)
                    {
                        workStep.TeamsByPolicy = teamNames;
                    }
                }

                // ✅ Resolve EventType names
                if (!string.IsNullOrWhiteSpace(incident.EventTypeIds))
                {
                    var ids = incident.EventTypeIds.Split(',').Select(long.Parse).ToList();
                    viewModel.incidentDetails.EventTypeNames = await _db.EventTypes
                        .Where(et => ids.Contains(et.Id))
                        .Select(et => et.Name)
                        .ToListAsync();
                }

                // ✅ Resolve Asset names
                if (!string.IsNullOrWhiteSpace(incident.AssetIds))
                {
                    var ids = incident.AssetIds.Split(',').Select(long.Parse).ToList();
                    viewModel.incidentiLocation.AssetNames = await _db.AssetIncidents
                        .Where(a => ids.Contains(a.Id))
                        .Select(a => a.Name)
                        .ToListAsync();
                }
                viewModel.workStepsByPolicy = viewModel.workSteps
                    .GroupBy(ws => ws.PolicyName)
                    .Select(g => g.ToList())  // each group is a list
                    .ToList(); // list of lists

                viewModel.IncidentViewRepairViewModel = validationRepairVM;
                viewModel.IncidentViewCloseoutViewModel = validationCloseoutVM;
                viewModel.IncidentViewRestorationViewModel = validationRestorationVM;
                viewModel.IncidentViewTaskViewModel = validationTaskVM;
                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentById.");
                return new IncidentViewModel();
            }
        }

        public async Task<GeocodeResult?> GetLatLngFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return new GeocodeResult
                {
                    Lat = 0,
                    Lng = 0
                };

            try
            {

                using var client = new HttpClient();
                string url = $"https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/findAddressCandidates" +
                             $"?f=json&SingleLine={Uri.EscapeDataString(address)}";

                var response = await client.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                var candidates = doc.RootElement.GetProperty("candidates");

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
                return new GeocodeResult
                {
                    Lat = 0,
                    Lng = 0
                };
            }
            return new GeocodeResult
            {
                Lat = 0,
                Lng = 0
            };
        }

        public async Task<List<IncidentLocationMapViewModel>> GetIncidentMapDetailsbyId(long incidentId)
        {
            List<IncidentLocationMapViewModel> locationMapViewModels = new();

            try
            {
                var incident = await _db.Incidents.Include(p => p.StatusLegend).Include(p => p.SeverityLevel)
                   .Where(i => !i.IsDeleted && i.Id == incidentId)
                   .FirstOrDefaultAsync();

                var incidentValidation = await _db.IncidentValidations
                   .Where(i => !i.IsDeleted && i.IncidentId == incidentId)
                   .FirstOrDefaultAsync();

                if (incident == null)
                {
                    return new List<IncidentLocationMapViewModel>();
                }

                locationMapViewModels.Add(new IncidentLocationMapViewModel
                {
                    lat = incident.Lat,
                    lon = incident.Lng,
                    severity = incident.SeverityLevel?.Name ?? string.Empty,
                    color = incident.SeverityLevel?.Color ?? string.Empty,
                    incidentloc = incident.LocationAddress ?? string.Empty,
                    calleraddress = incident.CallerAddress ?? string.Empty,
                    callername = incident.CallerName ?? string.Empty,
                    callerphone = incident.CallerPhoneNumber ?? string.Empty,
                    incidentid = incident.IncidentID,
                    assettype = await GetAssets(incident.AssetIds ?? string.Empty),
                    description = incident.DescriptionIssue ?? string.Empty,
                    eventtype = await GetEventTypes(incident.EventTypeIds ?? string.Empty),
                    intersection = incident.Landmark ?? string.Empty,
                    perimeter = incidentValidation != null ? GetPerimeter(incidentValidation.DiscoveryPerimeterId) : ""
                });

                return locationMapViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentMapDetailsbyId.");
                return new List<IncidentLocationMapViewModel>();
            }
        }

        public async Task<bool> SaveCommunicationMessage(SaveCommunicationRequest request)
        {
            try
            {

                var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdParsed = !string.IsNullOrEmpty(userId) ? long.Parse(userId) : 0;
                var userName = _httpContextAccessor?.HttpContext?.User.Identity?.Name ?? "Unknown User";

                var imageUrl = await SaveAttachments(request.File);
                var communicationHistory = new IncidentValidationCommunicationHistory
                {
                    IncidentId = request.IncidentId,
                    IncidentValidationId = request.IncidentValidationId,
                    UserName = userName,
                    Message = request.Message,
                    TimeStamp = DateTime.Now.ToString("MMM dd hh:mm tt"),
                    RecipientsIds = "",
                    ImageUrl = imageUrl ?? "",
                    MessageType = request.MessageType,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userIdParsed,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = userIdParsed,
                    IsDeleted = false,
                    ActiveStatus = Enums.ActiveStatus.Active
                };

                await _db.IncidentValidationCommunicationHistories.AddAsync(communicationHistory);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error SaveCommunicationMessage.");
                return false;
            }
        }
        public async Task<List<AdditionalLocationViewModel>> GetAdditionalLocationsByIncidentId(long incidentId)
        {
            try
            {
                var additionalLocations = await _db.AdditionalLocations
                    .Where(al => !al.IsDeleted && al.IncidentID.HasValue && al.IncidentID.Value == incidentId)
                    .OrderBy(al => al.Id)
                    .ToListAsync();

                var result = new List<AdditionalLocationViewModel>();

                foreach (var al in additionalLocations)
                {
                    var vm = new AdditionalLocationViewModel
                    {
                        Id = al.Id,
                        IncidentId = al.IncidentID,
                        LocationAddress = al.LocationAddress ?? string.Empty,
                        Latitude = al.Latitude,
                        Longitude = al.Longitude,
                        NearestIntersection = al.NearestIntersection ?? string.Empty,
                        ServiceAccount = al.ServiceAccount ?? string.Empty,
                        PerimeterType = al.PerimeterType,
                        PerimeterTypeDigit = al.PerimeterTypeDigit,
                        AssetIDs = al.AssetIds ?? string.Empty,
                        AssetNames = new List<string>(),
                        IsPrimaryLocation = al.IsPrimaryLocation
                    };

                    // Resolve asset names if AssetIds present (re-using your GetAssets helper)
                    if (!string.IsNullOrWhiteSpace(vm.AssetIDs))
                    {
                        vm.AssetNames = new List<string>(); //await GetAssets(vm.AssetIDs);
                    }

                    result.Add(vm);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAdditionalLocationsByIncidentId.");
                return new List<AdditionalLocationViewModel>();
            }
        }

        #region Map
        public async Task<long> AddMapChat(IncidentMapChatRequest request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var notes = new IncidentMapChat
                {
                    IncidentId = request.IncidentId,
                    ChatMessage = request.ChatMessage,
                    SentBy = request.SentBy,
                };

                _db.IncidentMapChats.Add(notes);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return notes.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error AddMapChat.");
                return 0;
            }
        }

        public async Task<List<IncidentMapChat>> GetIncidentMapChatChat(long incidentId)
        {
            List<IncidentMapChat> IncidentMapChats = new();

            try
            {
                IncidentMapChats = await _db.IncidentMapChats.Where(p => !p.IsDeleted && p.IncidentId == incidentId).ToListAsync();
                return IncidentMapChats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentMapChatChat.");
                return new List<IncidentMapChat>();
            }
        }
        #endregion

        #region Assestment
        public async Task<IncidentAssessmentDetailViewModel> GetAssessmentDetails(AssestmentFilterRequest request)
        {
            IncidentAssessmentDetailViewModel assessmentDetailViewModel = new();
            List<IncidentViewPostViewModel> ListPostDetailVM = new List<IncidentViewPostViewModel>();

            try
            {

                var statusList = await _db.Progress
                    .Where(p => !p.IsDeleted)
                    .ToDictionaryAsync(p => p.Id, p => p.Name);

                var ownerList = await _db.IncidentUsers
                    .Where(p => !p.IsDeleted && p.EmployeeType == "Supervisor")
                    .ToListAsync();

                var additionalLocations = await _db.AdditionalLocations
                  .Where(p => !p.IsDeleted && p.IncidentID == request.IncidentId)
                  .ToListAsync();

                var incidentUsers = await _db.IncidentUsers
                    .Where(p => !p.IsDeleted)
                    .ToDictionaryAsync(p => p.Id, p => new { p.FirstName, p.LastName });

                var details = await _db.IncidentValidationAssessments
                    .Where(p => !p.IsDeleted && p.IncidentId == request.IncidentId)
                    .FirstOrDefaultAsync();


                if (details == null)
                {
                    assessmentDetailViewModel.Status = statusList
                    .Select(p => new SelectListItem
                    {
                        Text = p.Value,
                        Value = p.Key.ToString()
                    })
                    .ToList();

                    assessmentDetailViewModel.OwenerTypes = ownerList
                        .Select(user => new SelectListItem
                        {
                            Text = $"{user.LastName} {user.FirstName}",
                            Value = user.Id.ToString()
                        })
                        .ToList();

                    return assessmentDetailViewModel;
                }
                ListPostDetailVM = await GetPostDetailVM(Convert.ToInt64(details?.IncidentId), 3);


                string GetUserFullName(long? userId) =>
                    userId.HasValue && incidentUsers.TryGetValue(userId.Value, out var user)
                        ? $"{user.LastName} {user.FirstName}"
                        : string.Empty;

                string GetStatusName(long? statusId) =>
                    statusId.HasValue && statusList.TryGetValue(statusId.Value, out var name)
                        ? name
                        : string.Empty;

                bool IsOwner(long? assignId) =>
                    assignId.HasValue && ownerList.Any(p => p.Id == assignId.Value);

                // Build all substep entries
                var incidentCommanderDetails = new List<IncidentCommanderDetailViewModel>
{
    new()
    {
        Mainstep = "Incident Commander",
        MainstepId = 1,
        Substep = "Create MCR",
        SubstepId = 1,
        StatusId = details.IC_MCR_StatusId,
        Status = GetStatusName(details.IC_MCR_StatusId),
        AssigneeId = details.IC_MCR_AssignId,
        Assignee = GetUserFullName(details.IC_MCR_AssignId),
        IsOwner = IsOwner(details.IC_MCR_AssignId),
        ClockIn = details.IC_MCR_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.IC_MCR_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.IC_MCR_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.IC_MCR_ImageUrls)
                        ? 0
                        : details.IC_MCR_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.IC_MCR_Notes
    },
    new()
    {
        Mainstep = "Incident Commander",
        MainstepId = 1,
        SubstepId = 2,
        Substep = "Notify Claims & Engineering",
        StatusId = details.IC_Notify_StatusId,
        Status = GetStatusName(details.IC_Notify_StatusId),
        AssigneeId = details.IC_Notify_AssignId,
        Assignee = GetUserFullName(details.IC_Notify_AssignId),
        IsOwner = IsOwner(details.IC_Notify_AssignId),
        ClockIn = details.IC_Notify_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.IC_Notify_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.IC_Notify_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.IC_Notify_ImageUrls)
                        ? 0
                        : details.IC_Notify_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.IC_Notify_Notes
    },
    new()
    {
        Mainstep = "Incident Commander",
        MainstepId = 1,
        SubstepId = 3,
        Substep = "Establish ICP (site access verified)",
        StatusId = details.IC_EstablishICP_StatusId,
        Status = GetStatusName(details.IC_EstablishICP_StatusId),
        AssigneeId = details.IC_EstablishICP_AssignId,
        Assignee = GetUserFullName(details.IC_EstablishICP_AssignId),
        IsOwner = IsOwner(details.IC_EstablishICP_AssignId),
        ClockIn = details.IC_EstablishICP_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.IC_EstablishICP_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.IC_EstablishICP_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.IC_EstablishICP_ImageUrls)
                        ? 0
                        : details.IC_EstablishICP_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.IC_EstablishICP_Notes
    },
    new()
    {
        Mainstep = "Field Environmental Representative",
        MainstepId = 2,
        SubstepId = 1,
        Substep = "Prepare containment area (drums/totes/Baker tank)",
        StatusId = details.FER_PCA_StatusId,
        Status = GetStatusName(details.FER_PCA_StatusId),
        AssigneeId = details.FER_PCA_AssignId,
        Assignee = GetUserFullName(details.FER_PCA_AssignId),
        IsOwner = IsOwner(details.FER_PCA_AssignId),
        ClockIn = details.FER_PCA_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.FER_PCA_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.FER_PCA_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.FER_PCA_ImageUrls)
                        ? 0
                        : details.FER_PCA_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.FER_PCA_Notes
    },
    new()
    {
        Mainstep = "Field Environmental Representative",
        MainstepId = 2,
        SubstepId = 2,
        Substep = "Label containers; log IDs and capacity",
        StatusId = details.FER_LC_StatusId,
        Status = GetStatusName(details.FER_LC_StatusId),
        AssigneeId = details.FER_LC_AssignId,
        Assignee = GetUserFullName(details.FER_LC_AssignId),
        IsOwner = IsOwner(details.FER_LC_AssignId),
        ClockIn = details.FER_LC_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.FER_LC_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.FER_LC_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.FER_LC_ImageUrls)
                        ? 0
                        : details.FER_LC_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.FER_LC_Notes
    },
    new()
    {
        Mainstep = "Engineering & GEC",
        MainstepId = 3,
        SubstepId = 1,
        Substep = "Retrieve system maps (regulators, BO streets, elevations)",
        StatusId = details.EGEC_RSM_StatusId,
        Status = GetStatusName(details.EGEC_RSM_StatusId),
        AssigneeId = details.EGEC_RSM_AssignId,
        Assignee = GetUserFullName(details.EGEC_RSM_AssignId),
        IsOwner = IsOwner(details.EGEC_RSM_AssignId),
        ClockIn = details.EGEC_RSM_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.EGEC_RSM_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.EGEC_RSM_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.EGEC_RSM_ImageUrls)
                        ? 0
                        : details.EGEC_RSM_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.EGEC_RSM_Notes
    },
    new()
    {
        Mainstep = "Engineering & GEC",
        MainstepId = 3,
        SubstepId = 2,
        Substep = "Mark low points & squeeze points",
        StatusId = details.EGEC_MLP_StatusId,
        Status = GetStatusName(details.EGEC_MLP_StatusId),
        AssigneeId = details.EGEC_MLP_AssignId,
        Assignee = GetUserFullName(details.EGEC_MLP_AssignId),
        IsOwner = IsOwner(details.EGEC_MLP_AssignId),
        ClockIn = details.EGEC_MLP_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.EGEC_MLP_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.EGEC_MLP_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.EGEC_MLP_ImageUrls)
                        ? 0
                        : details.EGEC_MLP_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.EGEC_MLP_Notes
    },
    new()
    {
        Mainstep = "Engineering & GEC",
        MainstepId = 3,
        SubstepId = 3,
        Substep = "Initiate cost tracking (RBA) for vendors",
        StatusId = details.EGEC_ICT_StatusId,
        Status = GetStatusName(details.EGEC_ICT_StatusId),
        AssigneeId = details.EGEC_ICT_AssignId,
        Assignee = GetUserFullName(details.EGEC_ICT_AssignId),
        IsOwner = IsOwner(details.EGEC_ICT_AssignId),
        ClockIn = details.EGEC_ICT_StartTime?.ToString("HH:mm") ?? "-",
        ClockOut = details.EGEC_ICT_ComplateTime?.ToString("HH:mm") ?? "-",
        ImagesUrl = details.EGEC_ICT_ImageUrls,
        ImageCount = string.IsNullOrWhiteSpace(details.EGEC_ICT_ImageUrls)
                        ? 0
                        : details.EGEC_ICT_ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
        Notes = details.EGEC_ICT_Notes
    }
};


                // --- 🧮 Calculate Task Counts ---
                var openTaskCount = incidentCommanderDetails.Count(x =>
                    x.Status != null && x.Status.Equals("In Progress", StringComparison.OrdinalIgnoreCase));

                var completedTaskCount = incidentCommanderDetails.Count(x =>
                    x.Status != null && x.Status.Equals("Done", StringComparison.OrdinalIgnoreCase));

                // --- 🔍 Apply Filters ---
                if (!string.IsNullOrWhiteSpace(request.step))
                {
                    incidentCommanderDetails = incidentCommanderDetails
                                             .Where(x => x.Substep != null &&
                                                         x.Substep.Contains(request.step, StringComparison.OrdinalIgnoreCase))
                                             .ToList();
                }

                if (request.ownerId > 0)
                {
                    incidentCommanderDetails = incidentCommanderDetails
                        .Where(x => x.AssigneeId == request.ownerId)
                        .ToList();
                }

                if (request.statusID > 0)
                {
                    incidentCommanderDetails = incidentCommanderDetails
                        .Where(x => x.StatusId == request.statusID)
                        .ToList();
                }

                // --- Build Owner Dictionary ---
                var ownersByMainStep = incidentCommanderDetails
                    .Where(x => x.IsOwner)
                    .GroupBy(x => x.Mainstep)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.Assignee).FirstOrDefault() ?? string.Empty
                    );

                // --- Map to ViewModel ---
                assessmentDetailViewModel.IncidentId = details.IncidentId;
                assessmentDetailViewModel.Id = details.Id;
                assessmentDetailViewModel.IncidentValidationId = details.IncidentValidationId;
                assessmentDetailViewModel.incidentCommanderDetailViewslist = incidentCommanderDetails;
                assessmentDetailViewModel.MainStepOwners = ownersByMainStep;

                assessmentDetailViewModel.Status = statusList
                    .Select(p => new SelectListItem
                    {
                        Text = p.Value,
                        Value = p.Key.ToString()
                    })
                    .ToList();

                assessmentDetailViewModel.OwenerTypes = ownerList
                    .Select(user => new SelectListItem
                    {
                        Text = $"{user.LastName} {user.FirstName}",
                        Value = user.Id.ToString()
                    })
                    .ToList();
                assessmentDetailViewModel.OpenTaskCount = openTaskCount;
                assessmentDetailViewModel.CompletedTaskCount = completedTaskCount;

                assessmentDetailViewModel.PrimaryLocationCount = additionalLocations.Count(p => p.IsPrimaryLocation);
                assessmentDetailViewModel.AdditionalLocationCount = additionalLocations.Count(p => !p.IsPrimaryLocation);
                assessmentDetailViewModel.ICPLocationCount = additionalLocations.Count;
                assessmentDetailViewModel.listIncidentViewPostViewModel = ListPostDetailVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAssessmentDetails for IncidentId: {IncidentId}", request.IncidentId);
                return new IncidentAssessmentDetailViewModel();
            }

            return assessmentDetailViewModel;
        }

        public async Task<IncidentAssessmentEditViewModel> EditAssessmentDetails(long id, long mainstepId, long substepId)
        {
            IncidentAssessmentEditViewModel editViewModel = new();

            try
            {
                // Fetch the incident assessment
                var details = await _db.IncidentValidationAssessments
                                       .Where(p => !p.IsDeleted && p.Id == id)
                                       .FirstOrDefaultAsync();

                var incidentUsers = await _db.IncidentUsers
                   .Where(p => !p.IsDeleted)
                   .ToDictionaryAsync(p => p.Id, p => new { p.FirstName, p.LastName });

                var statusList = await _db.Progress
                                .Where(p => !p.IsDeleted)
                                .ToDictionaryAsync(p => p.Id, p => p.Name);


                if (details == null)
                    return new IncidentAssessmentEditViewModel();



                // Build all substep entries
                var allSubsteps = new List<IncidentCommanderDetailViewModel>
                                {
                                    new() { Mainstep = "Incident Commander", MainstepId = 1, SubstepId = 1, Substep = "Create MCR", StatusId = details.IC_MCR_StatusId, AssigneeId = details.IC_MCR_AssignId },
                                    new() { Mainstep = "Incident Commander", MainstepId = 1, SubstepId = 2, Substep = "Notify Claims & Engineering", StatusId = details.IC_Notify_StatusId, AssigneeId = details.IC_Notify_AssignId },
                                    new() { Mainstep = "Incident Commander", MainstepId = 1, SubstepId = 3, Substep = "Establish ICP (site access verified)", StatusId = details.IC_EstablishICP_StatusId, AssigneeId = details.IC_EstablishICP_AssignId },
                                    new() { Mainstep = "Field Environmental Representative", MainstepId = 2, SubstepId = 1, Substep = "Prepare containment area (drums/totes/Baker tank)", StatusId = details.FER_PCA_StatusId, AssigneeId = details.FER_PCA_AssignId },
                                    new() { Mainstep = "Field Environmental Representative", MainstepId = 2, SubstepId = 2, Substep = "Label containers; log IDs and capacity", StatusId = details.FER_LC_StatusId, AssigneeId = details.FER_LC_AssignId },
                                    new() { Mainstep = "Engineering & GEC", MainstepId = 3, SubstepId = 1, Substep = "Retrieve system maps (regulators, BO streets, elevations)", StatusId = details.EGEC_RSM_StatusId, AssigneeId = details.EGEC_RSM_AssignId },
                                    new() { Mainstep = "Engineering & GEC", MainstepId = 3, SubstepId = 2, Substep = "Mark low points & squeeze points", StatusId = details.EGEC_MLP_StatusId, AssigneeId = details.EGEC_MLP_AssignId },
                                    new() { Mainstep = "Engineering & GEC", MainstepId = 3, SubstepId = 3, Substep = "Initiate cost tracking (RBA) for vendors", StatusId = details.EGEC_ICT_StatusId, AssigneeId = details.EGEC_ICT_AssignId }
                                };

                // Find the matching substep based on mainstepId and substepId
                var substep = allSubsteps.FirstOrDefault(s => s.MainstepId == mainstepId && s.SubstepId == substepId);

                if (substep != null)
                {
                    editViewModel = new IncidentAssessmentEditViewModel
                    {
                        StatusId = substep.StatusId,
                        AssigneeId = substep.AssigneeId,
                        MainStepId = substep.MainstepId,
                        SubStepId = substep.SubstepId,
                        Assignees = incidentUsers.Select(user => new SelectListItem
                        {
                            Text = $"{user.Value.LastName} {user.Value.FirstName}",
                            Value = user.Key.ToString()
                        }).ToList(),

                        Status = statusList
                            .Select(p => new SelectListItem
                            {
                                Text = p.Value,
                                Value = p.Key.ToString()
                            })
                            .ToList(),

                        MainStep = substep.Mainstep,
                        SubStep = substep.Substep,
                        Id = id
                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditAssessmentDetails for Id: {Id}, MainstepId: {mainstepId}, SubstepId: {substepId}", id, mainstepId, substepId);
                return new IncidentAssessmentEditViewModel();
            }

            return editViewModel;
        }
        public async Task<long> UpdateAssessment(IncidentAssessmentEditViewModel request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var details = await _db.IncidentValidationAssessments
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == request.Id);

                if (details == null)
                    return 0;

                // Time parsing helper
                DateTime? ParseTime(string time) =>
                    TimeSpan.TryParse(time, out var ts) ? DateTime.Today.Add(ts) : (DateTime?)null;

                // Step mapping configuration
                var stepMap = new Dictionary<(int mainStep, int subStep), Action>
                {
                    // Incident Commander
                    [(1, 1)] = () =>
                    {
                        details.IC_MCR_AssignId = request.AssigneeId;
                        details.IC_MCR_StatusId = request.StatusId;
                        details.IC_MCR_StartTime = ParseTime(request.StartedTime);
                        details.IC_MCR_ComplateTime = ParseTime(request.CompletedTime);
                        details.IC_MCR_Notes = request.Description;
                        details.IC_MCR_ImageUrls = request.ImageUrl;
                    },
                    [(1, 2)] = () =>
                    {
                        details.IC_Notify_AssignId = request.AssigneeId;
                        details.IC_Notify_StatusId = request.StatusId;
                        details.IC_Notify_StartTime = ParseTime(request.StartedTime);
                        details.IC_Notify_ComplateTime = ParseTime(request.CompletedTime);
                        details.IC_Notify_Notes = request.Description;
                        details.IC_Notify_ImageUrls = request.ImageUrl;
                    },
                    [(1, 3)] = () =>
                    {
                        details.IC_EstablishICP_AssignId = request.AssigneeId;
                        details.IC_EstablishICP_StatusId = request.StatusId;
                        details.IC_EstablishICP_StartTime = ParseTime(request.StartedTime);
                        details.IC_EstablishICP_ComplateTime = ParseTime(request.CompletedTime);
                        details.IC_EstablishICP_Notes = request.Description;
                        details.IC_EstablishICP_ImageUrls = request.ImageUrl;
                    },

                    // Field Environmental Representative
                    [(2, 1)] = () =>
                    {
                        details.FER_PCA_AssignId = request.AssigneeId;
                        details.FER_PCA_StatusId = request.StatusId;
                        details.FER_PCA_StartTime = ParseTime(request.StartedTime);
                        details.FER_PCA_ComplateTime = ParseTime(request.CompletedTime);
                        details.FER_PCA_Notes = request.Description;
                        details.FER_PCA_ImageUrls = request.ImageUrl;
                    },
                    [(2, 2)] = () =>
                    {
                        details.FER_LC_AssignId = request.AssigneeId;
                        details.FER_LC_StatusId = request.StatusId;
                        details.FER_LC_StartTime = ParseTime(request.StartedTime);
                        details.FER_LC_ComplateTime = ParseTime(request.CompletedTime);
                        details.FER_LC_Notes = request.Description;
                        details.FER_LC_ImageUrls = request.ImageUrl;
                    },

                    // Engineering & GEC
                    [(3, 1)] = () =>
                    {
                        details.EGEC_RSM_AssignId = request.AssigneeId;
                        details.EGEC_RSM_StatusId = request.StatusId;
                        details.EGEC_RSM_StartTime = ParseTime(request.StartedTime);
                        details.EGEC_RSM_ComplateTime = ParseTime(request.CompletedTime);
                        details.EGEC_RSM_Notes = request.Description;
                        details.EGEC_RSM_ImageUrls = request.ImageUrl;
                    },
                    [(3, 2)] = () =>
                    {
                        details.EGEC_MLP_AssignId = request.AssigneeId;
                        details.EGEC_MLP_StatusId = request.StatusId;
                        details.EGEC_MLP_StartTime = ParseTime(request.StartedTime);
                        details.EGEC_MLP_ComplateTime = ParseTime(request.CompletedTime);
                        details.EGEC_MLP_Notes = request.Description;
                        details.EGEC_MLP_ImageUrls = request.ImageUrl;
                    },
                    [(3, 3)] = () =>
                    {
                        details.EGEC_ICT_AssignId = request.AssigneeId;
                        details.EGEC_ICT_StatusId = request.StatusId;
                        details.EGEC_ICT_StartTime = ParseTime(request.StartedTime);
                        details.EGEC_ICT_ComplateTime = ParseTime(request.CompletedTime);
                        details.EGEC_ICT_Notes = request.Description;
                        details.EGEC_ICT_ImageUrls = request.ImageUrl;
                    }
                };

                // Execute correct mapping
                if (stepMap.TryGetValue(((int)request.MainStepId, (int)request.SubStepId), out var apply))
                    apply();
                else
                    return 0;

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return details.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating assessment");
                return 0;
            }
        }
        public async Task<IncidentAssessmentReadViewModel> ViewAssessmentDetails(long id, long mainstepId, long substepId)
        {
            IncidentAssessmentReadViewModel editViewModel = new();

            try
            {
                // Fetch the incident assessment
                var details = await _db.IncidentValidationAssessments
                                       .Where(p => !p.IsDeleted && p.Id == id)
                                       .FirstOrDefaultAsync();

                var incidentUsers = await _db.IncidentUsers
                   .Where(p => !p.IsDeleted)
                   .ToDictionaryAsync(p => p.Id, p => new { p.FirstName, p.LastName });

                var statusList = await _db.Progress
                                .Where(p => !p.IsDeleted)
                                .ToDictionaryAsync(p => p.Id, p => p.Name);

                string GetUserFullName(long? userId) =>
                   userId.HasValue && incidentUsers.TryGetValue(userId.Value, out var user)
                       ? $"{user.LastName} {user.FirstName}"
                       : string.Empty;

                string GetStatusName(long? statusId) =>
                    statusId.HasValue && statusList.TryGetValue(statusId.Value, out var name)
                        ? name
                        : string.Empty;

                if (details == null)
                    return new IncidentAssessmentReadViewModel();


                var allSubsteps = new List<IncidentCommanderDetailViewModel>
                    {
                        new()
                        {
                            Mainstep = "Incident Commander",
                            MainstepId = 1,
                            SubstepId = 1,
                            Substep = "Create MCR",
                            StatusId = details.IC_MCR_StatusId,
                            AssigneeId = details.IC_MCR_AssignId,
                            ImagesUrl = details.IC_MCR_ImageUrls,
                            ClockIn = details.IC_MCR_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.IC_MCR_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.IC_MCR_Notes,
                            Assignee = GetUserFullName(details.IC_MCR_AssignId),
                            Status = GetStatusName(details.IC_MCR_StatusId)
                        },
                        new()
                        {
                            Mainstep = "Incident Commander",
                            MainstepId = 1,
                            SubstepId = 2,
                            Substep = "Notify Claims & Engineering",
                            StatusId = details.IC_Notify_StatusId,
                            AssigneeId = details.IC_Notify_AssignId,
                            ImagesUrl = details.IC_Notify_ImageUrls,
                            ClockIn = details.IC_Notify_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.IC_Notify_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.IC_Notify_Notes,
                            Assignee = GetUserFullName(details.IC_Notify_AssignId),
                            Status = GetStatusName(details.IC_Notify_StatusId)
                        },
                        new()
                        {
                            Mainstep = "Incident Commander",
                            MainstepId = 1,
                            SubstepId = 3,
                            Substep = "Establish ICP (site access verified)",
                            StatusId = details.IC_EstablishICP_StatusId,
                            AssigneeId = details.IC_EstablishICP_AssignId,
                            ImagesUrl = details.IC_EstablishICP_ImageUrls,
                            ClockIn = details.IC_EstablishICP_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.IC_EstablishICP_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.IC_EstablishICP_Notes,
                            Assignee = GetUserFullName(details.IC_EstablishICP_AssignId),
                            Status = GetStatusName(details.IC_EstablishICP_StatusId)
                        },
                        new()
                        {
                            Mainstep = "Field Environmental Representative",
                            MainstepId = 2,
                            SubstepId = 1,
                            Substep = "Prepare containment area (drums/totes/Baker tank)",
                            StatusId = details.FER_PCA_StatusId,
                            AssigneeId = details.FER_PCA_AssignId,
                            ImagesUrl = details.FER_PCA_ImageUrls,
                            ClockIn = details.FER_PCA_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.FER_PCA_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.FER_PCA_Notes,
                            Assignee = GetUserFullName(details.FER_PCA_AssignId),
                            Status = GetStatusName(details.FER_PCA_StatusId)
                        },
                        new()
                        {
                            Mainstep = "Field Environmental Representative",
                            MainstepId = 2,
                            SubstepId = 2,
                            Substep = "Label containers; log IDs and capacity",
                            StatusId = details.FER_LC_StatusId,
                            AssigneeId = details.FER_LC_AssignId,
                            ImagesUrl = details.FER_LC_ImageUrls,
                            ClockIn = details.FER_LC_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.FER_LC_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.FER_LC_Notes,
                            Assignee = GetUserFullName(details.FER_LC_AssignId),
                            Status = GetStatusName(details.FER_LC_StatusId)
                        },
                        new()
                        {
                            Mainstep = "Engineering & GEC",
                            MainstepId = 3,
                            SubstepId = 1,
                            Substep = "Retrieve system maps (regulators, BO streets, elevations)",
                            StatusId = details.EGEC_RSM_StatusId,
                            AssigneeId = details.EGEC_RSM_AssignId,
                            ImagesUrl = details.EGEC_RSM_ImageUrls,
                            ClockIn = details.EGEC_RSM_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.EGEC_RSM_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.EGEC_RSM_Notes,
                            Assignee = GetUserFullName(details.EGEC_RSM_AssignId),
                            Status = GetStatusName(details.EGEC_RSM_StatusId)
                        },
                        new()
                        {
                            Mainstep = "Engineering & GEC",
                            MainstepId = 3,
                            SubstepId = 2,
                            Substep = "Mark low points & squeeze points",
                            StatusId = details.EGEC_MLP_StatusId,
                            AssigneeId = details.EGEC_MLP_AssignId,
                            ImagesUrl = details.EGEC_MLP_ImageUrls,
                            ClockIn = details.EGEC_MLP_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.EGEC_MLP_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.EGEC_MLP_Notes,
                            Assignee = GetUserFullName(details.EGEC_MLP_AssignId),
                            Status = GetStatusName(details.EGEC_MLP_StatusId)
                        },
                        new()
                        {
                            Mainstep = "Engineering & GEC",
                            MainstepId = 3,
                            SubstepId = 3,
                            Substep = "Initiate cost tracking (RBA) for vendors",
                            StatusId = details.EGEC_ICT_StatusId,
                            AssigneeId = details.EGEC_ICT_AssignId,
                            ImagesUrl = details.EGEC_ICT_ImageUrls,
                            ClockIn = details.EGEC_ICT_StartTime?.ToString("HH:mm") ?? "-",
                            ClockOut = details.EGEC_ICT_ComplateTime?.ToString("HH:mm") ?? "-",
                            Notes = details.EGEC_ICT_Notes,
                            Assignee = GetUserFullName(details.EGEC_ICT_AssignId),
                            Status = GetStatusName(details.EGEC_ICT_StatusId)
                        }
                    };

                // Find the matching substep based on mainstepId and substepId
                var substep = allSubsteps.FirstOrDefault(s => s.MainstepId == mainstepId && s.SubstepId == substepId);

                if (substep != null)
                {
                    editViewModel = new IncidentAssessmentReadViewModel
                    {
                        StatusId = substep.StatusId,
                        AssigneeId = substep.AssigneeId,
                        MainStepId = substep.MainstepId,
                        SubStepId = substep.SubstepId,

                        MainStep = substep.Mainstep,
                        SubStep = substep.Substep,
                        Id = id,

                        Description = substep.Notes,
                        ImageUrl = substep.ImagesUrl,
                        CompletedTime = substep.ClockOut,
                        StartedTime = substep.ClockIn,
                        Assignee = substep.Assignee,
                        Status = substep.Status
                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewAssessmentDetails for Id: {Id}, MainstepId: {mainstepId}, SubstepId: {substepId}", id, mainstepId, substepId);
                return new IncidentAssessmentReadViewModel();
            }

            return editViewModel;
        }
        public async Task<IncidentViewAssessmentAttachmentViewModel> ViewAssessmentAttachment(long id)
        {
            IncidentViewAssessmentAttachmentViewModel attachmentViewModel = new();

            try
            {
                // Fetch the incident assessment
                var details = await _db.IncidentValidationAssessments
                                       .Where(p => !p.IsDeleted && p.IncidentId == id)
                                       .FirstOrDefaultAsync();

                if (details != null)
                {
                    // Sab ImageUrls ko combine kar ke list banaye
                    var allImages = new List<string>();

                    void AddIfNotNullOrEmpty(string? urls)
                    {
                        if (!string.IsNullOrWhiteSpace(urls))
                        {
                            var splitUrls = urls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                .Select(u => Path.GetFileName(u.Trim())); // <-- sirf file name
                            allImages.AddRange(splitUrls);
                        }
                    }

                    // Har property call karo
                    AddIfNotNullOrEmpty(details.IC_MCR_ImageUrls);
                    AddIfNotNullOrEmpty(details.IC_Notify_ImageUrls);
                    AddIfNotNullOrEmpty(details.IC_EstablishICP_ImageUrls);
                    AddIfNotNullOrEmpty(details.FER_PCA_ImageUrls);
                    AddIfNotNullOrEmpty(details.FER_LC_ImageUrls);
                    AddIfNotNullOrEmpty(details.EGEC_RSM_ImageUrls);
                    AddIfNotNullOrEmpty(details.EGEC_MLP_ImageUrls);
                    AddIfNotNullOrEmpty(details.EGEC_ICT_ImageUrls);

                    // Result assign karo viewmodel me
                    attachmentViewModel.Image = allImages;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewAssessmentAttachment for Id: {Id}", id);
                return new IncidentViewAssessmentAttachmentViewModel();
            }

            return attachmentViewModel;
        }

        public async Task<IncidentAssessmentAddViewModel> AddAssessmentDetails()
        {
            IncidentAssessmentAddViewModel incidentAssessmentAddView = new();

            try
            {

                incidentAssessmentAddView.UserList = await _db.IncidentUsers
                                       .Where(it => !it.IsDeleted)
                                       .Select(it => new SelectListItem
                                       {
                                           Value = it.Id.ToString(),
                                           Text = it.FirstName + ' ' + it.LastName
                                       })
                                       .ToListAsync();

                incidentAssessmentAddView.StatusList = await _db.Progress
                                     .Where(it => !it.IsDeleted)
                                     .Select(it => new SelectListItem
                                     {
                                         Value = it.Id.ToString(),
                                         Text = it.Name
                                     })
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddAssessmentDetails for Id");
                return new IncidentAssessmentAddViewModel();
            }

            return incidentAssessmentAddView;

        }

        public async Task<long> SubmitAssestment(IncidentValidationAssessment request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                #region Incident Validation Assestment
                if (request != null)
                {
                    var incidentValidation = await _db.IncidentValidations.Where(p => p.IncidentId == request.IncidentId && !p.IsDeleted).FirstOrDefaultAsync();

                    request.IncidentValidationId = incidentValidation?.Id;
                    await _db.IncidentValidationAssessments.AddAsync(request);
                }
                #endregion

                #region  Save everything in one go
                // 6. Save everything in one go
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return request.Id;
                #endregion
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error SubmitAssestment.");
                return 0;
            }
        }

        #endregion

        public async Task<long> SaveValidationNoteAsync(SaveValidationNoteRequest request)
        {
            if (request == null || request.IncidentId <= 0 || string.IsNullOrWhiteSpace(request.Notes))
                return 0;

            try
            {
                var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdParsed = !string.IsNullOrEmpty(userId) ? long.Parse(userId) : 0;
                var userName = _httpContextAccessor?.HttpContext?.User.Identity?.Name ?? "Unknown";

                var note = new IncidentValidationNotes
                {
                    IncidentId = request.IncidentId,
                    IncidentValidationId = request.IncidentValidationId,
                    Notes = request.Notes,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userIdParsed,
                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = userIdParsed,
                    IsDeleted = false,
                    ActiveStatus = Enums.ActiveStatus.Active
                };

                await _db.IncidentValidationNotes.AddAsync(note);
                await _db.SaveChangesAsync();

                return note.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error SaveValidationNoteAsync.");
                return 0;
            }
        }

        #region private methods
        private bool TryParseCallTime(string callTime, out DateTime dateTime)
        {
            return DateTime.TryParse(callTime, out dateTime);
        }

        private string GetDate(string callTime)
        {
            if (TryParseCallTime(callTime, out var dt))
            {
                return dt.ToString("dd MMM, yyyy");  // Example: 29 Aug, 2025
            }
            return string.Empty;
        }

        private string GetTime(string callTime)
        {
            if (TryParseCallTime(callTime, out var dt))
            {
                return dt.ToString("HH:mm tt");      // Example: 02:53 PM
            }
            return string.Empty;
        }

        private string GetIndicator(long? value) =>
            value switch
            {
                1 => "Yes",
                0 => "No",
                2 => "N/A",
                _ => string.Empty
            };
        private string GetPerimeter(long? value) =>
           value switch
           {
               1 => "1 Mile",
               2 => "3 Miles",
               3 => "5 Miles",
               _ => string.Empty
           };
        private async Task<string> GetAssets(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return string.Empty;

            var idArray = ids.Split(",", StringSplitOptions.RemoveEmptyEntries)
                             .Select(id => long.TryParse(id.Trim(), out var val) ? val : (long?)null)
                             .Where(val => val.HasValue)
                             .Select(val => val.Value)
                             .ToList();

            var assetNames = await _db.AssetIncidents
                                      .Where(a => idArray.Contains(a.Id))
                                      .Select(a => a.Name)
                                      .ToListAsync();

            return string.Join(",", assetNames);
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
        private async Task<string> SaveAttachments(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return string.Empty;
            }

            var fileList = new List<string>();
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "incidents");

            // Ensure directory exists
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            foreach (var fileItem in files)
            {
                if (fileItem.Length <= 0) continue; // skip empty files

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileItem.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileItem.CopyToAsync(stream);
                }

                // Save relative path (for serving in browser)
                var relativePath = $"/Storage/uploads/incidents/{fileName}";
                fileList.Add(relativePath);

                _logger.LogInformation("Saved attachment: {FileName} at {Path}", fileName, relativePath);
            }

            // Return comma-separated list of paths
            return string.Join(",", fileList);
        }

        private string GetRegulatory(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return string.Empty;

            var idArray = new[] { "1", "2", "3", "4", "5" };
            var assetNames = new[] { "CPOC", "PHMSA", "NTSB", "EPA", "Local Fire Dept" };

            var inputIds = ids.Split(',').Select(x => x.Trim()).ToList();

            var selected = idArray
                .Select((id, index) => new { id, name = assetNames[index] })
                .Where(x => inputIds.Contains(x.id))
                .Select(x => x.name);

            return string.Join(",", selected);
        }
        #endregion

        #region Personnel
        public async Task<List<SelectListItem>> GetAllUsersDrop()
        {
            try
            {
                var users = await _db.IncidentUsers
                    .Where(c => !c.IsDeleted)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.FirstName + " " + c.LastName
                    })
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Users.");
                return new List<SelectListItem>();
            }
        }
        public async Task<List<SelectListItem>> GetAllCompaniesDrop()
        {
            try
            {
                var company = await _db.Company
                    .Where(c => !c.IsDeleted)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToListAsync();

                return company;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Companies.");
                return new List<SelectListItem>();
            }
        }
        public async Task<List<SelectListItem>> GetAllIncidentRolesDrop()
        {
            try
            {
                var roles = await _db.IncidentRoles
                    .Where(c => !c.IsDeleted)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToListAsync();
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Roles.");
                return new List<SelectListItem>();
            }
        }
        public async Task<List<SelectListItem>> GetAllShiftsDrop()
        {
            try
            {
                var shifts = await _db.IncidentShifts
                    .Where(c => !c.IsDeleted)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToListAsync();
                return shifts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Shifts.");
                return new List<SelectListItem>();
            }
        }
        public async Task<List<IncidentViewModel.CompanyViewModel>> GetAllCompanies()
        {
            try
            {
                var companies = await _db.Company
                    .Where(c => !c.IsDeleted)
                    .Select(c => new IncidentViewModel.CompanyViewModel
                    {
                        CompanyId = c.Id,
                        CompanyName = c.Name
                    })
                    .ToListAsync();
                return companies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All Companies.");
                return new List<IncidentViewModel.CompanyViewModel>();
            }
        }
        public async Task<List<IncidentViewModel.IncidentRoleViewModel>> GetAllIncidentRoles()
        {
            try
            {
                var roles = await _db.IncidentRoles
                    .Where(r => !r.IsDeleted)
                    .Select(r => new IncidentViewModel.IncidentRoleViewModel
                    {
                        IncidentRoleId = r.Id,
                        IncidentRoleName = r.Name
                    })
                    .ToListAsync();
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get All IncidentRoles.");
                return new List<IncidentViewModel.IncidentRoleViewModel>();
            }
        }
        public async Task<List<IncidentViewModel.ProgressStatusViewModel>> GetAllProgressStatus()
        {
            try
            {
                var statuses = await _db.Progress
                    .Where(s => !s.IsDeleted)
                    .Select(s => new IncidentViewModel.ProgressStatusViewModel
                    {
                        StatusId = s.Id,
                        StatusName = s.Name
                    })
                    .ToListAsync();

                return statuses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all Progress Statuses.");
                return new List<IncidentViewModel.ProgressStatusViewModel>();
            }
        }

        public async Task<IncidentValidationPersonnelsCountViewModel> UpdateTimeIn(long Id, DateTime timeIn)
        {
            try
            {
                // Get the personnel record
                var personnel = await _db.IncidentValidationPersonnels.FirstOrDefaultAsync(p => p.Id == Id);
                if (personnel == null)
                {
                    return null; // or new IncidentValidationPersonnelsCountViewModel() if you prefer
                }

                long? incidentId = personnel.IncidentId;
                await using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    personnel.TimeIn = timeIn;
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                var now = DateTime.Now;

                // Get all personnels for this IncidentId
                var IncidentValidationPersonnels = (
                    from ivp in _db.IncidentValidationPersonnels
                    where ivp.IncidentId == incidentId
                    join u in _db.IncidentUsers on ivp.UserId equals u.Id into userGroup
                    from u in userGroup.DefaultIfEmpty()
                    join c in _db.Company on ivp.CompanyId equals c.Id into companyGroup
                    from c in companyGroup.DefaultIfEmpty()
                    join r in _db.IncidentRoles on ivp.RoleId equals r.Id into roleGroup
                    from r in roleGroup.DefaultIfEmpty()
                    join s in _db.IncidentShifts on ivp.ShiftId equals s.Id into shiftGroup
                    from s in shiftGroup.DefaultIfEmpty()
                    join si in _db.IncidentUsers on ivp.SupervisorId equals si.Id into supervisorGroup
                    from si in supervisorGroup.DefaultIfEmpty()
                    select new IncidentValidationPersonnelsViewModel
                    {
                        IncidentValidationPersonnelsId = ivp.Id,
                        UserId = ivp.UserId,
                        CompanyId = ivp.CompanyId,
                        Name = (u.FirstName + " " + u.LastName).Trim(),
                        Company = c.Name,
                        Role = r.Name,
                        Type = u.EmployeeType,
                        Shift = s.Name,
                        TimeIn = ivp.TimeIn,
                        Supervisor = (si.FirstName + " " + si.LastName).Trim()
                    }
                ).ToList();

                // Stats
                var OnsiteNow = await _db.IncidentValidationPersonnels.CountAsync(p => p.IncidentId == incidentId);
                var CheckedOutToday = await _db.IncidentValidationPersonnels.CountAsync(p =>
                    p.IncidentId == incidentId && p.TimeIn.HasValue && p.TimeIn.Value.Date == now.Date);

                var personnelTimeToday = await _db.IncidentValidationPersonnels
                    .Where(p => !p.IsDeleted && p.IncidentId == incidentId && p.TimeIn.HasValue && p.TimeIn.Value.Date == now.Date)
                    .ToListAsync();

                var totalHoursToday = Math.Round(personnelTimeToday.Sum(p => (now - p.TimeIn.Value).TotalHours), 2);

                var TotalDayShift = IncidentValidationPersonnels.Count(ds => ds.Shift == "Day Shift");
                var TotalNightShift = IncidentValidationPersonnels.Count(ds => ds.Shift == "Night Shift");
                var TotalEmployees = IncidentValidationPersonnels.Count(ds => ds.Type == "Employee");
                var TotalContractors = IncidentValidationPersonnels.Count(ds => ds.Type == "Contractor");

                double AvgHoursWorker = CheckedOutToday > 0 ? Math.Round(totalHoursToday / CheckedOutToday, 2) : 0;

                var incidentValidationPersonnelsCountViewModel = new IncidentValidationPersonnelsCountViewModel
                {
                    OnsiteNowCount = OnsiteNow,
                    CheckedOutTodayCount = CheckedOutToday,
                    TotalHoursToday = totalHoursToday,
                    AvgHoursWorker = AvgHoursWorker,
                    TotalDayShift = TotalDayShift,
                    TotalNightShift = TotalNightShift,
                    TotalEmployees = TotalEmployees,
                    TotalContractors = TotalContractors,
                    IncidentId = Id,
                };

                return incidentValidationPersonnelsCountViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update TimeIn.");
                return null;
            }
        }
        public async Task<List<IncidentValidationPersonnelsViewModel>> GetFilterByRole(long incidentId, long roleId, long companyid, string onsite)
        {
            try
            {
                bool isOnsite = string.Equals(onsite, "true", StringComparison.OrdinalIgnoreCase);

                DateTime today = DateTime.Today;

                var query = _db.IncidentValidationPersonnels
                    .Where(p => p.IncidentId == incidentId
                                && p.RoleId == (roleId == 0 ? p.RoleId : roleId)
                                && p.CompanyId == (companyid == 0 ? p.CompanyId : companyid)
                                && !p.IsDeleted);

                if (isOnsite)
                {
                    query = query.Where(p => p.TimeIn.HasValue && p.TimeIn.Value.Date == today);
                }

                var filteredData = query
                    .GroupJoin(_db.IncidentUsers,
                               ivp => ivp.UserId,
                               u => u.Id,
                               (ivp, userGroup) => new { ivp, userGroup })
                    .SelectMany(x => x.userGroup.DefaultIfEmpty(), (x, u) => new { x.ivp, u })
                    .GroupJoin(_db.Company,
                               x => x.ivp.CompanyId,
                               c => c.Id,
                               (x, companyGroup) => new { x.ivp, x.u, companyGroup })
                    .SelectMany(x => x.companyGroup.DefaultIfEmpty(), (x, c) => new { x.ivp, x.u, c })
                    .GroupJoin(_db.IncidentRoles,
                               x => x.ivp.RoleId,
                               r => r.Id,
                               (x, roleGroup) => new { x.ivp, x.u, x.c, roleGroup })
                    .SelectMany(x => x.roleGroup.DefaultIfEmpty(), (x, r) => new { x.ivp, x.u, x.c, r })
                    .GroupJoin(_db.IncidentShifts,
                               x => x.ivp.ShiftId,
                               s => s.Id,
                               (x, shiftGroup) => new { x.ivp, x.u, x.c, x.r, shiftGroup })
                    .SelectMany(x => x.shiftGroup.DefaultIfEmpty(), (x, s) => new { x.ivp, x.u, x.c, x.r, s })
                    .GroupJoin(_db.IncidentUsers, // Supervisor
                               x => x.ivp.SupervisorId,
                               sup => sup.Id,
                               (x, supervisorGroup) => new { x.ivp, x.u, x.c, x.r, x.s, supervisorGroup })
                    .SelectMany(x => x.supervisorGroup.DefaultIfEmpty(), (x, sup) => new IncidentValidationPersonnelsViewModel
                    {
                        IncidentValidationPersonnelsId = x.ivp.Id,
                        UserId = x.ivp.UserId,
                        CompanyId = x.ivp.CompanyId,
                        Name = x.u != null ? (x.u.FirstName + " " + x.u.LastName).Trim() : string.Empty,
                        Company = x.c != null ? x.c.Name : string.Empty,
                        Role = x.r != null ? x.r.Name : string.Empty,
                        Type = x.u != null ? x.u.EmployeeType : string.Empty,
                        Shift = x.s != null ? x.s.Name : string.Empty,
                        Supervisor = sup != null ? (sup.FirstName + " " + sup.LastName).Trim() : string.Empty,
                        TimeIn = x.ivp.TimeIn
                    })
                    .ToList();

                return filteredData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get Filter By Role.");
                return new List<IncidentValidationPersonnelsViewModel>();
            }
        }
        public async Task<List<IncidentViewModel.UsersViewModel>> GetSupervisors(long companyId, long userId)
        {
            try
            {
                var users = await _db.IncidentUsers
                    .Where(u => !u.IsDeleted && u.CompanyId == companyId && u.Id != userId)
                    .Select(u => new IncidentViewModel.UsersViewModel
                    {
                        UsersId = u.Id,
                        UsersName = (u.FirstName + " " + u.LastName).Trim()
                    })
                    .ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get Supervisors.");
                return new List<IncidentViewModel.UsersViewModel>();
            }
        }
        public async Task<long> UpdateSupervisor(long personnelId, long supervisorId)
        {
            try
            {

                var UpdateSupervisor = await _db.IncidentValidationPersonnels.Where(p => p.Id == personnelId).FirstOrDefaultAsync();

                if (UpdateSupervisor == null)
                {
                    return 0;
                }

                await using var transaction = await _db.Database.BeginTransactionAsync();

                UpdateSupervisor.SupervisorId = supervisorId;

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return UpdateSupervisor.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update Supervisor.");
                return 0;
            }
        }
        public async Task<IncidentValidationPersonnelsCountViewModel> AddPerson(long userId, long companyId, long roleId, long shiftId, long incidentId, long incidentValidationId)
        {
            try
            {
                await using var transaction = await _db.Database.BeginTransactionAsync();

                var incidentValidation = await _db.IncidentValidations
                   .Where(i => !i.IsDeleted && i.IncidentId == incidentId)
                   .FirstOrDefaultAsync();
                var newPersonnel = new IncidentValidationPersonnel
                {
                    IncidentId = incidentId,
                    UserId = userId,
                    CompanyId = companyId,
                    RoleId = roleId,
                    ShiftId = shiftId,
                    IncidentValidationId = incidentValidation.Id, // if this is a foreign key
                    TimeIn = null, // set if needed
                };

                _db.IncidentValidationPersonnels.Add(newPersonnel);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var now = DateTime.Now;

                var IncidentValidationPersonnels = await (
                    from ivp in _db.IncidentValidationPersonnels
                    where ivp.IncidentId == incidentId
                    join u in _db.IncidentUsers on ivp.UserId equals u.Id into userGroup
                    from u in userGroup.DefaultIfEmpty()
                    join c in _db.Company on ivp.CompanyId equals c.Id into companyGroup
                    from c in companyGroup.DefaultIfEmpty()
                    join r in _db.IncidentRoles on ivp.RoleId equals r.Id into roleGroup
                    from r in roleGroup.DefaultIfEmpty()
                    join s in _db.IncidentShifts on ivp.ShiftId equals s.Id into shiftGroup
                    from s in shiftGroup.DefaultIfEmpty()
                    join si in _db.IncidentUsers on ivp.SupervisorId equals si.Id into supervisorGroup
                    from si in supervisorGroup.DefaultIfEmpty()
                    select new IncidentValidationPersonnelsViewModel
                    {
                        IncidentValidationPersonnelsId = ivp.Id,
                        UserId = ivp.UserId,
                        CompanyId = ivp.CompanyId,
                        Name = (u.FirstName + " " + u.LastName).Trim(),
                        Company = c.Name,
                        Role = r.Name,
                        Type = u.EmployeeType,
                        Shift = s.Name,
                        TimeIn = ivp.TimeIn,
                        Supervisor = (si.FirstName + " " + si.LastName).Trim()
                    }
                ).ToListAsync();

                // Stats
                var OnsiteNow = await _db.IncidentValidationPersonnels.CountAsync(p => p.IncidentId == incidentId);
                var CheckedOutToday = await _db.IncidentValidationPersonnels.CountAsync(p =>
                    p.IncidentId == incidentId && p.TimeIn.HasValue && p.TimeIn.Value.Date == now.Date);

                var personnelTimeToday = await _db.IncidentValidationPersonnels
                    .Where(p => !p.IsDeleted && p.IncidentId == incidentId && p.TimeIn.HasValue && p.TimeIn.Value.Date == now.Date)
                    .ToListAsync();

                var totalHoursToday = Math.Round(personnelTimeToday.Sum(p => (now - p.TimeIn.Value).TotalHours), 2);

                var TotalDayShift = IncidentValidationPersonnels.Count(ds => ds.Shift == "Day Shift");
                var TotalNightShift = IncidentValidationPersonnels.Count(ds => ds.Shift == "Night Shift");
                var TotalEmployees = IncidentValidationPersonnels.Count(ds => ds.Type == "Employee");
                var TotalContractors = IncidentValidationPersonnels.Count(ds => ds.Type == "Contractor");

                double AvgHoursWorker = CheckedOutToday > 0 ? Math.Round(totalHoursToday / CheckedOutToday, 2) : 0;

                return new IncidentValidationPersonnelsCountViewModel
                {
                    OnsiteNowCount = OnsiteNow,
                    CheckedOutTodayCount = CheckedOutToday,
                    TotalHoursToday = totalHoursToday,
                    AvgHoursWorker = AvgHoursWorker,
                    TotalDayShift = TotalDayShift,
                    TotalNightShift = TotalNightShift,
                    TotalEmployees = TotalEmployees,
                    TotalContractors = TotalContractors,
                    IncidentId = incidentId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting IncidentValidationPersonnels record.");
                return null;
            }
        }
        #endregion

        #region IncidentValidationRepair
        public async Task<List<IncidentViewCloseoutListViewModel>> GetvalidationCloseoutVM(long id)
        {
            var roles = await _db.IncidentRoles.ToListAsync();
            var repairs = await _db.ValidationCloseouts
                .Where(p => !p.IsDeleted && p.IncidentId == id)
                .ToListAsync();
            var statuses = await _db.Progress.ToListAsync(); // Your status table
            var OnsiteNow = await _db.IncidentValidationPersonnels.CountAsync(p => p.IncidentId == id);

            // Step 2️⃣: Build final projected list
            var result = repairs
                .Select(x => new
                {
                    x.Id,
                    x.IncidentId,
                    x.IncidentValidationId,
                    x.Description,
                    x.Role,
                    // Convert comma-separated IDs to comma-separated role names
                    FieldValue = string.Join(" / ",
                        (x.Role ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => v.Trim())
                            .Select(v => long.TryParse(v, out var vid) ? roles.FirstOrDefault(r => r.Id == vid)?.Name : null)
                            .Where(name => !string.IsNullOrEmpty(name))
                    ),
                    x.Status
                })
                .OrderBy(x => x.Id)
                .ToList();


            return result.Select(p => new IncidentViewCloseoutListViewModel
            {
                Id = p.Id,
                IncidentId = p.IncidentId,
                IncidentValidationId = p.IncidentValidationId,
                Task = p.Description,
                FieldValue = p.FieldValue ?? string.Empty,
                Status = !string.IsNullOrWhiteSpace(p.Status) ? statuses.FirstOrDefault(s => s.Id == Convert.ToInt64(p.Status))?.Name : ""
            }).ToList();
        }

        public async Task<List<IncidentViewPostViewModel>> GetPostDetailVM(long id, long IncidentViewType)
        {
            var PostDetails = await _db.IncidentPostDetails
               .Where(p => !p.IsDeleted && p.IncidentId == id && p.IncidentViewType == IncidentViewType)
               .ToListAsync();
            //List <IncidentViewPostViewModel> result = new List <IncidentViewPostViewModel>();
            //return result.Select(p => new IncidentViewPostViewModel
            //{
            //    Id = p.Id,
            //    IncidentId = p.IncidentId,
            //    IncidentValidationId = p.IncidentValidationId

            //}).ToList();

            var result = new List<IncidentViewPostViewModel>
            {
                //new IncidentViewPostViewModel
                //{
                //    Id = 1,
                //    IncidentId = id,
                //    //IncidentValidationId = 1001,
                //    TimeforMessage = "11:42 IC",
                //    Message = "ICP-1 purge completed (2,800 gal extracted)."
                //},
                //new IncidentViewPostViewModel
                //{
                //    Id = 2,
                //    IncidentId = id,
                //    //IncidentValidationId = 1002,
                //    TimeforMessage = "12:45 FER",
                //    Message = "ICP-2 purge verified and logged."
                //},
                //new IncidentViewPostViewModel
                //{
                //    Id = 3,
                //    IncidentId = id,
                //    //IncidentValidationId = 1003,
                //    TimeforMessage = "13:10 ENG",
                //    Message = "Pressure gauge readings stable."
                //},
                //new IncidentViewPostViewModel
                //{
                //    Id = 4,
                //    IncidentId = id,
                //    //IncidentValidationId = 1004,
                //    TimeforMessage = "13:25 GEC",
                //    Message = "Disposal manifest issued for PRG-001."
                //},
                //new IncidentViewPostViewModel
                //{
                //    Id = 5,
                //    IncidentId = id,
                //    //IncidentValidationId = 1005,
                //    TimeforMessage = "13:35 FER",
                //    Message = "Containment labels verified for Baker tank."
                //}
            };

            return PostDetails.Select(p => new IncidentViewPostViewModel
            {
                Id = p.Id,
                IncidentId = p.IncidentId,
                Message = p.Message,
                TimeforMessage = p.MessageTime,
                IncidentViewType = p.IncidentViewType
            }).ToList();
        }

        public async Task<List<IncidentViewPostViewModel>> SavePostDetails(IncidentViewPostViewModel incidentViewPostViewModel)
        {

            var IncidentPostDetail = new IncidentPostDetail
            {
                IncidentId = incidentViewPostViewModel.IncidentId,
                Message = incidentViewPostViewModel.Message,
                MessageTime = incidentViewPostViewModel.TimeforMessage,
                IncidentViewType = incidentViewPostViewModel.IncidentViewType,
                ActiveStatus = Enums.ActiveStatus.Active
            };
            // Save
            await _db.IncidentPostDetails.AddAsync(IncidentPostDetail);
            await _db.SaveChangesAsync();

            List<IncidentViewPostViewModel> listIncidentViewPostViewModel = await GetPostDetailVM(incidentViewPostViewModel.IncidentId, incidentViewPostViewModel.IncidentViewType);
            return listIncidentViewPostViewModel;
        }
        #endregion
        public async Task<List<IncidentViewTaskListViewModel>> GetvalidationTaskVM(long incidentId)
        {
            // lookup tables
            var roles = await _db.IncidentRoles.AsNoTracking().ToListAsync();
            var statuses = await _db.Progress.AsNoTracking().ToListAsync(); // status table

            // tasks table (columns shown in your screenshot)
            var tasks = await _db.IncidentValidationTasks
                .AsNoTracking()
                .Where(t => !t.IsDeleted && t.IncidentId == incidentId)
                .OrderBy(t => t.Id)
                .ToListAsync();

            var result = tasks
                .Select(x => new
                {
                    x.Id,
                    x.IncidentId,
                    x.IncidentValidationId,
                    TaskDescription = x.TaskDescription,
                    RoleIds = x.RoleIds,
                    StatusId = x.StatusId,
                    CreatedOn = x.CreatedOn,
                    StartTime = x.StartTime,
                    x.ComplateTime,
                    x.ImageUrls,
                    x.Notes
                })
                .ToList();

            return result.Select(p =>
            {
                // Resolve RoleIds -> Names (tolerant to nulls/non-numeric tokens)
                var responsible = string.Join(" / ",
                    (p.RoleIds ?? string.Empty)
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Select(s => long.TryParse(s, out var rid)
                            ? roles.FirstOrDefault(r => r.Id == rid)?.Name
                            : null)
                        .Where(n => !string.IsNullOrEmpty(n))
                );

                var statusName = p.StatusId.HasValue
                    ? statuses.FirstOrDefault(s => s.Id == p.StatusId.Value)?.Name ?? string.Empty
                    : string.Empty;

                return new IncidentViewTaskListViewModel
                {
                    Id = p.Id,
                    IncidentId = p.IncidentId ?? 0,
                    IncidentValidationId = p.IncidentValidationId ?? 0,
                    Task = p.TaskDescription ?? string.Empty,
                    FieldValue = string.IsNullOrWhiteSpace(responsible) ? "—" : responsible,
                    Status = string.IsNullOrWhiteSpace(statusName) ? "Pending" : statusName,

                    Started = p.StartTime?.ToString("HH:mm") ?? "-",
                    Completed = p.ComplateTime?.ToString("HH:mm") ?? "-",
                    ImagesUrl = p?.ImageUrls ?? string.Empty,
                    ImageCount = string.IsNullOrWhiteSpace(p?.ImageUrls ?? string.Empty)
                        ? 0
                        : p.ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
                    Notes = p?.Notes ?? string.Empty
                };
            }).ToList();
        }
        public async Task<IncidentViewTaskListViewModel> AddIncidentTaskAsync(AddIncidentTaskRequest request)
        {
            var entity = new IncidentValidationTask
            {
                IncidentId = request.IncidentId,
                IncidentValidationId = request.IncidentValidationId,
                TaskDescription = request.TaskDescription,
                RoleIds = request.RoleIds, // comma-separated "1,2"
                StatusId = request.StatusId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            _db.IncidentValidationTasks.Add(entity);
            await _db.SaveChangesAsync();

            // load role names and status name (similar to GetvalidationTaskVM logic)
            var roles = await _db.IncidentRoles.ToListAsync();
            var statuses = await _db.Progress.ToListAsync();

            var roleNames = (entity.RoleIds ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => long.TryParse(s, out var id) ? roles.FirstOrDefault(r => r.Id == id)?.Name : null)
                .Where(n => !string.IsNullOrEmpty(n));

            var statusName = entity.StatusId.HasValue ? statuses.FirstOrDefault(s => s.Id == entity.StatusId.Value)?.Name : null;

            return new IncidentViewTaskListViewModel
            {
                Id = entity.Id,
                IncidentId = entity.IncidentId,
                IncidentValidationId = entity.IncidentValidationId,
                Task = entity.TaskDescription,
                FieldValue = roleNames.Any() ? string.Join(" / ", roleNames) : "—",
                Status = string.IsNullOrWhiteSpace(statusName) ? "Pending" : statusName,
                Started = null,
                Completed = null,
                Attachment = null
            };
        }

        #region Restoration
        public async Task<IncidentEditTaskListViewModel> EditRestorationDetails(long id)
        {
            IncidentEditTaskListViewModel editViewModel = new();

            try
            {
                // Fetch the incident assessment
                var details = await _db.IncidentValidationTasks
                                       .Where(p => !p.IsDeleted && p.Id == id)
                                       .FirstOrDefaultAsync();

                var roles = await _db.IncidentRoles.Where(p => !p.IsDeleted).AsNoTracking().ToListAsync() ?? new List<IncidentRole>();

                var statusList = await _db.Progress
                                .Where(p => !p.IsDeleted)
                                .ToDictionaryAsync(p => p.Id, p => p.Name) ?? new Dictionary<long, string>();


                editViewModel = new IncidentEditTaskListViewModel
                {
                    Id = details?.Id ?? 0,
                    IncidentId = details?.IncidentId ?? 0,
                    IncidentValidationId = details?.IncidentValidationId ?? 0,
                    Task = details?.TaskDescription ?? string.Empty,
                    RoleIds = details?.RoleIds ?? string.Empty,
                    StatusId = details?.StatusId,
                    StatusList = statusList
                    .Select(p => new SelectListItem
                    {
                        Text = p.Value,
                        Value = p.Key.ToString()
                    })
                    .ToList(),
                    RoleList = roles
                    .Select(p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    })
                    .ToList(),
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditRestorationDetails");
                return new IncidentEditTaskListViewModel();
            }

            return editViewModel;
        }

        public async Task<long> UpdateRestoration(IncidentEditTaskListViewModel request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var details = await _db.IncidentValidationTasks
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == request.Id);


                if (details == null)
                    return 0;

                //var validation = await _db.IncidentValidations
                //   .FirstOrDefaultAsync(v => !v.IsDeleted && v.Id == details.IncidentId);


                // 🕒 Helper to parse time like "10:30"
                DateTime? ParseTime(string time) =>
                    TimeSpan.TryParse(time, out var ts) ? DateTime.Today.Add(ts) : (DateTime?)null;

                // 🔹 Update existing entity fields
                //details.IncidentId = request.IncidentId;
                //details.IncidentValidationId = validation?.Id ?? 0;
                details.TaskDescription = request?.Task ?? string.Empty;
                details.RoleIds = request?.RoleIds ?? string.Empty; // comma-separated "1,2"
                details.StatusId = request?.StatusId ?? 0;
                details.ImageUrls = request?.ImageUrl ?? string.Empty;
                details.StartTime = ParseTime(request?.Started ?? string.Empty);
                details.ComplateTime = ParseTime(request?.Completed ?? string.Empty);
                details.Notes = request?.Description;
                details.UpdatedOn = DateTime.UtcNow; // optional if you track update time

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                }
                return details.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating restoration task");
                return 0;
            }
        }

        public async Task<IncidentViewAssessmentAttachmentViewModel> ViewRestorationAttachment(long id)
        {
            IncidentViewAssessmentAttachmentViewModel attachmentViewModel = new();

            try
            {
                // Fetch the incident assessment
                var details = await _db.IncidentValidationTasks
                                       .Where(p => !p.IsDeleted && p.IncidentId == id)
                                       .ToListAsync();

                if (details.Count > 0)
                {
                    // Sab ImageUrls ko combine kar ke list banaye
                    var allImages = new List<string>();

                    foreach (var item in details)
                    {
                        void AddIfNotNullOrEmpty(string? urls)
                        {
                            if (!string.IsNullOrWhiteSpace(urls))
                            {
                                var splitUrls = urls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(u => Path.GetFileName(u.Trim())); // <-- sirf file name
                                allImages.AddRange(splitUrls);
                            }
                        }
                        // Har property call karo
                        AddIfNotNullOrEmpty(item.ImageUrls);
                    }

                    // Result assign karo viewmodel me
                    attachmentViewModel.Image = allImages;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewRestorationAttachment for Id: {Id}", id);
                return new IncidentViewAssessmentAttachmentViewModel();
            }

            return attachmentViewModel;
        }

        public async Task<IncidentEditTaskListViewModel> ViewRestorationDetails(long id)
        {
            IncidentEditTaskListViewModel editViewModel = new();

            try
            {
                var details = await _db.IncidentValidationTasks
                    .Where(p => !p.IsDeleted && p.Id == id)
                    .FirstOrDefaultAsync();

                if (details == null)
                    return new IncidentEditTaskListViewModel();

                var statusList = await _db.Progress
                    .Where(p => !p.IsDeleted)
                    .ToDictionaryAsync(p => p.Id, p => p.Name);

                var roles = await _db.IncidentRoles
                    .Where(p => !p.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync() ?? new List<IncidentRole>();

                string GetStatusName(long? statusId) =>
                    statusId.HasValue && statusList.TryGetValue(statusId.Value, out var name)
                        ? name
                        : string.Empty;

                // 🔹 Convert stored comma-separated RoleIds into readable Role Names
                string roleNames = string.Empty;
                if (!string.IsNullOrWhiteSpace(details.RoleIds))
                {
                    var roleIds = details.RoleIds.Split(',')
                                                 .Select(idStr => long.TryParse(idStr, out var idVal) ? idVal : 0)
                                                 .Where(idVal => idVal > 0)
                                                 .ToList();

                    roleNames = string.Join(", ",
                        roles.Where(r => roleIds.Contains(r.Id))
                             .Select(r => r.Name)
                             .ToList());
                }

                // 🔹 Prepare ViewModel
                editViewModel = new IncidentEditTaskListViewModel
                {
                    Id = id,
                    IncidentId = details.IncidentId,
                    IncidentValidationId = details.IncidentValidationId,
                    Description = details.Notes ?? string.Empty,
                    ImageUrl = details.ImageUrls ?? string.Empty,
                    Completed = details.ComplateTime?.ToString("HH:mm") ?? "-",
                    Started = details.StartTime?.ToString("HH:mm") ?? "-",
                    StatusId = details.StatusId,
                    RoleIds = details.RoleIds, // still keep raw IDs
                    RoleName = roleNames,      // ✅ comma-separated role names
                    StatusName = GetStatusName(details.StatusId),
                    Task = details.TaskDescription
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewRestorationDetails");
                return new IncidentEditTaskListViewModel();
            }

            return editViewModel;
        }

        //public async Task<IncidentAssessmentAddViewModel> AddRestorationDetails()
        //{
        //    IncidentAssessmentAddViewModel incidentAssessmentAddView = new();

        //    try
        //    {

        //        incidentAssessmentAddView.UserList = await _db.IncidentUsers
        //                               .Where(it => !it.IsDeleted)
        //                               .Select(it => new SelectListItem
        //                               {
        //                                   Value = it.Id.ToString(),
        //                                   Text = it.FirstName + ' ' + it.LastName
        //                               })
        //                               .ToListAsync();

        //        incidentAssessmentAddView.StatusList = await _db.Progress
        //                             .Where(it => !it.IsDeleted)
        //                             .Select(it => new SelectListItem
        //                             {
        //                                 Value = it.Id.ToString(),
        //                                 Text = it.Name
        //                             })
        //                             .ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in AddAssessmentDetails for Id");
        //        return new IncidentAssessmentAddViewModel();
        //    }

        //    return incidentAssessmentAddView;

        //}
        #endregion

        #region ClouseOut
        public async Task<IncidentViewTaskListViewModel> AddIncidentTaskCloseOutAsync(AddIncidentTaskRequest request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = new ValidationCloseout
                {
                    IncidentId = request?.IncidentId ?? 0,
                    IncidentValidationId = request?.IncidentValidationId ?? 0,
                    Description = request?.TaskDescription,
                    Role = request?.RoleIds, // comma-separated "1,2"
                    Status = request?.StatusId.ToString(),
                };

                _db.ValidationCloseouts.Add(entity);

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                }

                // load role names and status name (similar to GetvalidationTaskVM logic)
                var roles = await _db.IncidentRoles.ToListAsync();
                var statuses = await _db.Progress.ToListAsync();

                var roleNames = (entity.Role ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => long.TryParse(s, out var id) ? roles.FirstOrDefault(r => r.Id == id)?.Name : null)
                    .Where(n => !string.IsNullOrEmpty(n));

                var statusName = !string.IsNullOrWhiteSpace(entity.Status) ? statuses.FirstOrDefault(s => s.Id == Convert.ToInt64(entity.Status))?.Name : null;

                return new IncidentViewTaskListViewModel
                {
                    Id = entity.Id,
                    IncidentId = entity.IncidentId,
                    IncidentValidationId = entity.IncidentValidationId,
                    Task = entity.Description,
                    FieldValue = roleNames.Any() ? string.Join(" / ", roleNames) : "—",
                    Status = string.IsNullOrWhiteSpace(statusName) ? "Not Started" : statusName,
                    Started = null,
                    Completed = null,
                    Attachment = null
                };

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error Update ClouseOut task");
                return new IncidentViewTaskListViewModel();
            }
        }
        public async Task<IncidentEditTaskListViewModel> EditClouseOutDetails(long id)
        {
            IncidentEditTaskListViewModel editViewModel = new();

            try
            {
                // Fetch the incident assessment
                var details = await _db.ValidationCloseouts
                                       .Where(p => !p.IsDeleted && p.Id == id)
                                       .FirstOrDefaultAsync();

                var roles = await _db.IncidentRoles.Where(p => !p.IsDeleted).AsNoTracking().ToListAsync() ?? new List<IncidentRole>();

                var statusList = await _db.Progress
                                .Where(p => !p.IsDeleted)
                                .ToDictionaryAsync(p => p.Id, p => p.Name) ?? new Dictionary<long, string>();


                editViewModel = new IncidentEditTaskListViewModel
                {
                    Id = details?.Id ?? 0,
                    IncidentId = details?.IncidentId ?? 0,
                    IncidentValidationId = details?.IncidentValidationId ?? 0,
                    Task = details?.Description ?? string.Empty,
                    RoleIds = details?.Role ?? string.Empty,
                    StatusId = !string.IsNullOrWhiteSpace(details?.Status) ? Convert.ToInt64(details?.Status) : 0,
                    StatusList = statusList
                    .Select(p => new SelectListItem
                    {
                        Text = p.Value,
                        Value = p.Key.ToString()
                    })
                    .ToList(),
                    RoleList = roles
                    .Select(p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    })
                    .ToList(),
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditClouseOutDetails");
                return new IncidentEditTaskListViewModel();
            }

            return editViewModel;
        }
        public async Task<List<IncidentViewTaskListViewModel>> GetvalidationTaskClouseOut(long incidentId)
        {
            // lookup tables
            var roles = await _db.IncidentRoles.AsNoTracking().ToListAsync();
            var statuses = await _db.Progress.AsNoTracking().ToListAsync(); // status table

            try
            {
                // tasks table (columns shown in your screenshot)
                var tasks = await _db.ValidationCloseouts
                    .AsNoTracking()
                    .Where(t => !t.IsDeleted && t.IncidentId == incidentId)
                    .OrderBy(t => t.Id)
                    .ToListAsync();

                var result = tasks
                    .Select(x => new
                    {
                        x.Id,
                        x.IncidentId,
                        x.IncidentValidationId,
                        TaskDescription = x.Description,
                        RoleIds = x.Role,
                        StatusId = x.Status,
                        CreatedOn = x.CreatedOn,
                        StartTime = x.StartTime,
                        x.ComplateTime,
                        x.ImageUrls,
                        x.Notes
                    })
                    .ToList();

                return result.Select(p =>
                {
                    // Resolve RoleIds -> Names (tolerant to nulls/non-numeric tokens)
                    var responsible = string.Join(" / ",
                        (p.RoleIds ?? string.Empty)
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Select(s => long.TryParse(s, out var rid)
                                ? roles.FirstOrDefault(r => r.Id == rid)?.Name
                                : null)
                            .Where(n => !string.IsNullOrEmpty(n))
                    );

                    long parsedId;
                    var statusName = long.TryParse(p.StatusId, out parsedId)
                        ? statuses.FirstOrDefault(s => s.Id == parsedId)?.Name ?? string.Empty
                        : string.Empty;


                    return new IncidentViewTaskListViewModel
                    {
                        Id = p.Id, // assuming already long
                        IncidentId = p?.IncidentId ?? 0,
                        IncidentValidationId = p?.IncidentValidationId ?? 0,
                        Task = p.TaskDescription ?? string.Empty,
                        FieldValue = string.IsNullOrWhiteSpace(responsible) ? "—" : responsible,
                        Status = string.IsNullOrWhiteSpace(statusName) ? "Not Started" : statusName,

                        Started = p.StartTime?.ToString("HH:mm") ?? "-",
                        Completed = p.ComplateTime?.ToString("HH:mm") ?? "-",
                        ImagesUrl = p?.ImageUrls ?? string.Empty,
                        ImageCount = string.IsNullOrWhiteSpace(p?.ImageUrls ?? string.Empty)
            ? 0
            : p.ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries).Length,
                        Notes = p?.Notes ?? string.Empty
                    };

                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update ClouseOut task");
                return new List<IncidentViewTaskListViewModel>();
            }
        }
        public async Task<long> UpdateClouseOut(IncidentEditTaskListViewModel request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var details = await _db.ValidationCloseouts
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == request.Id);


                if (details == null)
                    return 0;


                // 🕒 Helper to parse time like "10:30"
                DateTime? ParseTime(string time) =>
                    TimeSpan.TryParse(time, out var ts) ? DateTime.Today.Add(ts) : (DateTime?)null;

                // 🔹 Update existing entity fields
                details.Description = request?.Task ?? string.Empty;
                details.Role = request?.RoleIds ?? string.Empty; // comma-separated "1,2"
                details.Status = Convert.ToString(request?.StatusId);
                details.ImageUrls = request?.ImageUrl ?? string.Empty;
                details.StartTime = ParseTime(request?.Started ?? string.Empty);
                details.ComplateTime = ParseTime(request?.Completed ?? string.Empty);
                details.Notes = request?.Description;
                details.UpdatedOn = DateTime.UtcNow; // optional if you track update time

                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                }
                return details.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error Update ClouseOut task");
                return 0;
            }
        }

        public async Task<IncidentViewAssessmentAttachmentViewModel> ViewClouseOutAttachment(long id)
        {
            IncidentViewAssessmentAttachmentViewModel attachmentViewModel = new();

            try
            {
                // Fetch the incident assessment
                var details = await _db.ValidationCloseouts
                                       .Where(p => !p.IsDeleted && p.IncidentId == id)
                                       .ToListAsync();

                if (details.Count > 0)
                {
                    // Sab ImageUrls ko combine kar ke list banaye
                    var allImages = new List<string>();

                    foreach (var item in details)
                    {
                        void AddIfNotNullOrEmpty(string? urls)
                        {
                            if (!string.IsNullOrWhiteSpace(urls))
                            {
                                var splitUrls = urls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(u => Path.GetFileName(u.Trim())); // <-- sirf file name
                                allImages.AddRange(splitUrls);
                            }
                        }
                        // Har property call karo
                        AddIfNotNullOrEmpty(item.ImageUrls);
                    }

                    // Result assign karo viewmodel me
                    attachmentViewModel.Image = allImages;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewRestorationAttachment for Id: {Id}", id);
                return new IncidentViewAssessmentAttachmentViewModel();
            }

            return attachmentViewModel;
        }

        public async Task<IncidentEditTaskListViewModel> ViewClouseOutDetails(long id)
        {
            IncidentEditTaskListViewModel editViewModel = new();

            try
            {
                var details = await _db.ValidationCloseouts
                    .Where(p => !p.IsDeleted && p.Id == id)
                    .FirstOrDefaultAsync();

                if (details == null)
                    return new IncidentEditTaskListViewModel();

                var statusList = await _db.Progress
                    .Where(p => !p.IsDeleted)
                    .ToDictionaryAsync(p => p.Id, p => p.Name);

                var roles = await _db.IncidentRoles
                    .Where(p => !p.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync() ?? new List<IncidentRole>();

                string GetStatusName(long? statusId) =>
                    statusId.HasValue && statusList.TryGetValue(statusId.Value, out var name)
                        ? name
                        : string.Empty;

                // 🔹 Convert stored comma-separated RoleIds into readable Role Names
                string roleNames = string.Empty;
                if (!string.IsNullOrWhiteSpace(details.Role))
                {
                    var roleIds = details.Role.Split(',')
                                                 .Select(idStr => long.TryParse(idStr, out var idVal) ? idVal : 0)
                                                 .Where(idVal => idVal > 0)
                                                 .ToList();

                    roleNames = string.Join(", ",
                        roles.Where(r => roleIds.Contains(r.Id))
                             .Select(r => r.Name)
                             .ToList());
                }

                // 🔹 Prepare ViewModel
                editViewModel = new IncidentEditTaskListViewModel
                {
                    Id = id,
                    IncidentId = details.IncidentId,
                    IncidentValidationId = details.IncidentValidationId,
                    Description = details.Notes ?? string.Empty,
                    ImageUrl = details.ImageUrls ?? string.Empty,
                    Completed = details.ComplateTime?.ToString("HH:mm") ?? "-",
                    Started = details.StartTime?.ToString("HH:mm") ?? "-",
                    StatusId = !string.IsNullOrWhiteSpace(details.Status) ? Convert.ToInt64(details.Status) : 0,
                    RoleIds = details.Role, // still keep raw IDs
                    RoleName = roleNames,      // ✅ comma-separated role names
                    StatusName = !string.IsNullOrWhiteSpace(details.Status) ? GetStatusName(Convert.ToInt64(details.Status)) : "Not Started",
                    Task = details.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewRestorationDetails");
                return new IncidentEditTaskListViewModel();
            }

            return editViewModel;
        }

        public async Task<long> GetTaskClouseOutCompletedCount(long incidentId)
        {
            long totalCount = 0;

            try
            {
                // Get all statuses (Id → Name)
                var statusList = await _db.Progress
                    .Where(p => !p.IsDeleted)
                    .ToDictionaryAsync(p => p.Id, p => p.Name)
                    ?? new Dictionary<long, string>();

                // Get all status IDs whose name is "Done" (case-insensitive)
                var doneStatusIds = statusList
                    .Where(x => string.Equals(x.Value, "Done", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Key)
                    .ToList();

                // Count tasks that belong to this incident and have a "Done" status
                totalCount = await _db.ValidationCloseouts
                    .AsNoTracking()
                    .Where(t => !t.IsDeleted
                                && t.IncidentId == incidentId
                                && doneStatusIds.Contains(Convert.ToInt64(t.Status)))
                    .LongCountAsync();

                return totalCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CloseOut completed task count");
                return totalCount;
            }
        }
        #endregion

        #region Repair
        public async Task<List<IncidentViewRepairListViewModel>> GetvalidationRepairVM(long id)
        {

            var roles = await _db.IncidentRoles.ToListAsync();
            var statuses = await _db.Progress.ToListAsync();
            var repairs = await _db.IncidentValidationRepairs
                .Where(p => !p.IsDeleted && p.IncidentId == id)
                .ToListAsync();

            // Create base template (3 rows)
            var baseTemplate = new[]
            {
        new { FieldTypeId = 1,
              FieldType = "Use \"Identifying Source of Leak\" Checklist (Pg. 4)",
              GetValue = new Func<IncidentValidationRepair, string?>(r => r?.SourceOfLeak),
              GetStatus = new Func<IncidentValidationRepair, string?>(r => r?.SourceOfLeakStatus) },

        new { FieldTypeId = 2,
              FieldType = "Identify ideal purge locations to prevent further outage (use engineering data)",
              GetValue = new Func<IncidentValidationRepair, string?>(r => r?.PreventFurtherOutage),
              GetStatus = new Func<IncidentValidationRepair, string?>(r => r?.PreventFurtherOutageStatus) },

        new { FieldTypeId = 3,
              FieldType = "Verify vacuum truck fittings (2\" cam-lock and 2\" → ¾\" adaptors available)",
              GetValue = new Func<IncidentValidationRepair, string?>(r => r?.VacuumTruckFitting),
              GetStatus = new Func<IncidentValidationRepair, string?>(r => r?.VacuumTruckFittingStatus) }
    };

            // If no data in DB, still show 3 rows
            var result = (repairs.Any() ? repairs : new List<IncidentValidationRepair> { new IncidentValidationRepair { IncidentId = id } })
                .SelectMany(r => baseTemplate.Select(t => new
                {
                    r.Id,
                    r.IncidentId,
                    r.IncidentValidationId,
                    FieldTypeId = t.FieldTypeId,
                    FieldType = t.FieldType,
                    FieldValue = t.GetValue(r),
                    FieldStatus = t.GetStatus(r)
                }))
                // Replace IDs with names
                .Select(x => new
                {
                    x.Id,
                    x.IncidentId,
                    x.IncidentValidationId,
                    x.FieldType,
                    x.FieldTypeId,
                    FieldValue = string.Join(" / ",
                        (x.FieldValue ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => v.Trim())
                            .Select(v => long.TryParse(v, out var vid) ? roles.FirstOrDefault(r => r.Id == vid)?.Name : null)
                            .Where(name => !string.IsNullOrEmpty(name))
                    ),
                    x.FieldStatus
                })
                .OrderBy(x => x.Id)
                .ToList();

            // Map to ViewModel
            var viewModelList = result.Select(p => new IncidentViewRepairListViewModel
            {
                Id = p.Id,
                IncidentId = p.IncidentId,
                IncidentValidationId = p.IncidentValidationId,
                FieldTypeId = p.FieldTypeId,
                FieldType = (p.FieldType == "" ? "-" : p.FieldType),
                FieldValue = (p.FieldValue == "" ? "-" : p.FieldValue ?? string.Empty),
                FieldStatus = statuses.FirstOrDefault(s => s.Id == Convert.ToInt64(p.FieldStatus ?? "0"))?.Name ?? string.Empty
            }).ToList();

            return viewModelList;
        }
        public async Task<IncidentRepairEditViewModel> EditRepairDetails(long id, long RepairId, long FieldType, long IncidentId, long IncidentValidationId)
        {
            IncidentRepairEditViewModel editViewModel = new();

            try
            {
                // Fetch the incident repair
                List<IncidentViewRepairListViewModel> ListvalidationRepairVM = await GetvalidationRepairVM(RepairId);


                var incidentUsers = await _db.IncidentUsers
                   .Where(p => !p.IsDeleted)
                   .ToDictionaryAsync(p => p.Id, p => new { p.FirstName, p.LastName });

                var statusList = await _db.Progress
                                .Where(p => !p.IsDeleted)
                                .ToDictionaryAsync(p => p.Id, p => p.Name);
                var rolesList = await _db.IncidentRoles
                                   .Where(it => !it.IsDeleted)
                                   .Select(it => new SelectListItem
                                   {
                                       Value = it.Id.ToString(),
                                       Text = it.Name
                                   })
                                   .ToListAsync();
                //if (details == null)
                //    return new IncidentRepairEditViewModel();


                editViewModel = ListvalidationRepairVM.Where(i => i.FieldTypeId == FieldType)
                    .Select(i => new IncidentRepairEditViewModel
                    {
                        //Assignees = incidentUsers.Select(user => new SelectListItem
                        //{
                        //    Text = $"{user.Value.LastName} {user.Value.FirstName}",
                        //    Value = user.Key.ToString()
                        //}).ToList(),

                        Status = statusList
                            .Select(p => new SelectListItem
                            {
                                Text = p.Value,
                                Value = p.Key.ToString()
                            })
                            .ToList(),
                        IncidentId = IncidentId,
                        IncidentValidationId = IncidentValidationId,
                        RoleList = rolesList,
                        FieldType = i.FieldType,
                        FieldTypeId = i.FieldTypeId,
                        Id = RepairId
                    }).FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditRepairDetails for Id: {Id}, RepairId: {RepairId}, FieldType: {FieldType}", id, RepairId, FieldType);
                return new IncidentRepairEditViewModel();
            }

            return editViewModel;
        }
        public async Task<long> UpdateRepair(IncidentRepairEditViewModel request)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var details = await _db.IncidentValidationRepairs
                    .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == request.Id);


                if (details == null || details.Id == 0)
                {
                    var IncidentValidationRepair = new IncidentValidationRepair
                    {
                        IncidentValidationId = Convert.ToInt64(request.IncidentValidationId),
                        IncidentId = Convert.ToInt64(request.IncidentId),
                        SourceOfLeak = request.SourceOfLeak,
                        SourceOfLeakStatus = request.SourceOfLeakStatus,
                        PreventFurtherOutage = request.PreventFurtherOutage,
                        PreventFurtherOutageStatus = request.PreventFurtherOutageStatus,
                        VacuumTruckFitting = request.VacuumTruckFitting,
                        VacuumTruckFittingStatus = request.VacuumTruckFittingStatus,
                        SOL_Path = request.SOL_Path,
                        SOL_Remark = request.SOL_Remark,
                        PFO_Path = request.PFO_Path,
                        PFO_Remark = request.PFO_Remark,
                        VTF_Path = request.VTF_Path,
                        VTF_Remark = request.VTF_Remark,
                        ActiveStatus = ActiveStatus.Active
                    };
                    await _db.IncidentValidationRepairs.AddAsync(IncidentValidationRepair);
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return IncidentValidationRepair.Id;
                }
                else
                {
                    if (request.FieldTypeId == 1)
                    {
                        details.SourceOfLeak = request.SourceOfLeak;
                        details.SourceOfLeakStatus = request.SourceOfLeakStatus;
                        details.SOL_Path = request.SOL_Path;
                        details.SOL_Remark = request.SOL_Remark;
                    }
                    else if (request.FieldTypeId == 2)
                    {
                        details.PreventFurtherOutage = request.PreventFurtherOutage;
                        details.PreventFurtherOutageStatus = request.PreventFurtherOutageStatus;
                        details.PFO_Path = request.PFO_Path;
                        details.PFO_Remark = request.PFO_Remark;
                    }
                    else if (request.FieldTypeId == 3)
                    {
                        details.VacuumTruckFitting = request.VacuumTruckFitting;
                        details.VacuumTruckFittingStatus = request.VacuumTruckFittingStatus;
                        details.VTF_Path = request.VTF_Path;
                        details.VTF_Remark = request.VTF_Remark;
                    }
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return details.Id;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating Repair");
                return 0;
            }
        }
        #endregion

    }
}
