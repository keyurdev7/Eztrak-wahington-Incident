using AutoMapper;

using Azure;

using Centangle.Common.ResponseHelpers;
using Centangle.Common.ResponseHelpers.Models;

using DataLibrary;

using DocumentFormat.OpenXml.InkML;
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

using Repositories.Shared.UserInfoServices.Interface;

using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using ViewModels;
using ViewModels.Dashboard;
using ViewModels.Incident;
using ViewModels.Shared;

namespace Repositories.Common
{
    public class IncidentDashboardService : IIncidentDashboardService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<IncidentDashboardService> _logger;

        public IncidentDashboardService(ApplicationDbContext db, ILogger<IncidentDashboardService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<DashboardViewModel> GetIncidentDashboardReportOld()
        {
            try
            {
                var incidents = await _db.Incidents.Where(i => !i.IsDeleted).ToListAsync();
                var incidentsStatus = await _db.Incidents.Include(p => p.StatusLegend).Where(i => !i.IsDeleted).ToListAsync();

                var severityData = incidents
                    .GroupBy(i => i.SeverityLevelId)
                    .Select(g => new
                    {
                        Id = g.Key,
                        Count = g.Count()
                    })
                    .Join(_db.SeverityLevels,
                          g => g.Id,
                          s => s.Id,
                          (g, s) => new IncidentDashboardSeverityReportViewModel
                          {
                              color = s.Color,
                              name = s.Name,
                              count = g.Count
                          })
                    .ToList();

                var statusData = incidents
                    .GroupBy(i => i.StatusLegendId)
                    .Select(g => new
                    {
                        Id = g.Key,
                        Count = g.Count()
                    })
                    .Join(_db.StatusLegends,
                          g => g.Id,
                          s => s.Id,
                          (g, s) => new IncidentDashboardStatusReportViewModel
                          {
                              color = s.Color,
                              name = s.Name,
                              count = g.Count
                          })
                    .ToList();



                // Define static colors for events
                var eventColors = new Dictionary<int, string>
                                {
                                    { 1, "#FF0000" }, // Red
                                    { 2, "#00FF00" }, // Green
                                    { 3, "#0000FF" }, // Blue
                                    { 4, "#FFA500" }, // Orange
                                    { 5, "#800080" }, // Purple
                                    { 6, "#008080" }, // Teal
                                    { 7, "#808000" }, // Olive
                                    { 10, "#FFD700" }, // Gold
                                    { 11, "#A52A2A" }  // Brown
                                };

                var eventData = incidents
                                .SelectMany(i => (i.EventTypeIds ?? "")
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(id => new { EventId = int.Parse(id), Incident = i }))
                                .GroupBy(x => x.EventId)
                                 .Select(g => new
                                 {
                                     Id = g.Key,
                                     Count = g.Count()
                                 })
                                 .Join(_db.EventTypes,
                                     g => g.Id,
                                     e => e.Id,
                                     (g, e) => new IncidentDashboardEventTypeReportViewModel
                                     {
                                         color = eventColors.ContainsKey(g.Id) ? eventColors[g.Id] : "#808080",
                                         name = e.Name,
                                         count = g.Count
                                     })
                                 .ToList();

                var assetTypeData = incidents
                                    .SelectMany(i => (i.AssetIds ?? "")
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(id => new { AssetId = int.Parse(id), Incident = i }))
                                .GroupBy(x => x.AssetId)
                                .Select(g => new
                                {
                                    Id = g.Key,
                                    Count = g.Count()
                                })
                                .Join(_db.AssetIncidents,
                                    g => g.Id,
                                    e => e.Id,
                                    (g, e) => new IncidentDashboardAssetTypeReportViewModel
                                    {
                                        name = e.Name,
                                        count = g.Count,
                                        color = eventColors.ContainsKey(g.Id) ? eventColors[g.Id] : "#808080"
                                    })
                                .ToList();


                var totalIncidentCount = incidents.Count;

                var totalSeverity = severityData.Sum(x => x.count);
                var totalStatus = statusData.Sum(x => x.count);
                var totalEvent = eventData.Sum(x => x.count);
                var totalAssetType = assetTypeData.Sum(x => x.count);

                foreach (var s in severityData)
                {
                    s.SeverityPercentage = totalSeverity == 0
                        ? 0
                        : Math.Round((decimal)s.count / totalSeverity * 100, 2);
                }

                foreach (var s in statusData)
                {
                    s.StatusPercentage = totalStatus == 0
                        ? 0
                        : Math.Round((decimal)s.count / totalStatus * 100, 2);
                }

                foreach (var s in eventData)
                {
                    s.EventTypePercentage = totalEvent == 0
                        ? 0
                        : Math.Round((decimal)s.count / totalEvent * 100, 2);
                }

                foreach (var s in assetTypeData)
                {
                    s.AssetTypePercentage = totalAssetType == 0
                        ? 0
                        : Math.Round((decimal)s.count / totalAssetType * 100, 2);
                }

                return new DashboardViewModel
                {
                    IncidentDashboard = new IncidentDashboardViewModel
                    {
                        SeverityLabels = severityData.Select(s => s.name).ToList(),
                        SeverityCounts = severityData.Select(s => s.count).ToList(),
                        SeverityColors = severityData.Select(s => s.color).ToList(),
                        StatusLabels = statusData.Select(s => s.name).ToList(),
                        StatusCounts = statusData.Select(s => s.count).ToList(),
                        StatusColors = statusData.Select(s => s.color).ToList(),
                        ListIncidentDashboardSeverityReportViewModel = severityData,
                        ListIncidentDashboardStatusReport = statusData,
                        ListIncidentDashboardEventTypeReportViewModel = eventData,
                        ListIncidentDashboardAssetTypeReportViewModel = assetTypeData,
                        TotalIncidentCount = totalIncidentCount,
                        TotalSeverityCount = totalSeverity,
                        TotalStatusLegendCount = totalStatus,
                        TotalEventTypeCount = totalEvent,
                        TotalAssetTypeCount = totalAssetType,
                        TotalSubmittedCount = incidentsStatus.Count(i => i.StatusLegend.Name == StatusLegendEnum.Submitted.ToString()),
                        TotalValidatedCount = incidentsStatus.Count(i => i.StatusLegend.Name == StatusLegendEnum.Validated.ToString()),
                        //TotalDispatchedCount = incidentsStatus.Count(i => i.StatusLegend.Name == StatusLegendEnum.Dispatched.ToString()),
                        TotalCompletedCount = incidentsStatus.Count(i => i.StatusLegend.Name == StatusLegendEnum.Completed.ToString()),
                        TotalCancelledCount = incidentsStatus.Count(i => i.StatusLegend.Name == StatusLegendEnum.Cancelled.ToString()),
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentDashboardReport.");
                return new DashboardViewModel();
            }
        }

        public async Task<DashboardViewModel> GetIncidentDashboardReport()
        {
            try
            {
                List<IncidentRecentViewModel> incidentRecents = new();

                var assetIncidents = await _db.AssetIncidents.ToListAsync();

                var incidentValidations = await _db.IncidentValidations
                 .Where(i => !i.IsDeleted).ToListAsync();

                var eventTypes = await _db.EventTypes.ToListAsync();
                // Pull incidents in memory (only once)
                var incidents = await _db.Incidents.Include(p => p.StatusLegend).Include(p => p.SeverityLevel)
                    .Where(i => !i.IsDeleted)
                    .ToListAsync();
                foreach (var item in incidents.OrderByDescending(p => p.Id).Take(5).ToList())
                {
                    incidentRecents.Add(new IncidentRecentViewModel()
                    {
                        eventtype = GetEventTypes(eventTypes, item.EventTypeIds ?? string.Empty),
                        incidentId = item.IncidentID,
                        incidentloc = item.LocationAddress ?? string.Empty,
                        incidentstatus = item.StatusLegend?.Name ?? string.Empty,
                        incidentstatusColor = item.StatusLegend?.Color ?? string.Empty,
                        severity = item.SeverityLevel?.Name ?? string.Empty,
                        lat = item.Lat,
                        lon = item.Lng,
                        perimeter = incidentValidations.Count > 0 ? GetPerimeter(incidentValidations.Where(i => i.IncidentId == item.Id).FirstOrDefault()?.DiscoveryPerimeterId) : "",
                        assettype = GetAssets(assetIncidents, item.AssetIds ?? string.Empty),
                        description = item.DescriptionIssue ?? string.Empty,
                        intersection = item.Landmark ?? string.Empty
                    });
                }

                // Preload lookup tables in memory (avoid multiple DB hits)
                var severityLevels = await _db.SeverityLevels.ToListAsync();
                var statusLegends = await _db.StatusLegends.ToListAsync();





                var incidentLocation = incidents.Where(p => p.StatusLegend?.Name != StatusLegendEnum.Completed.ToString() && p.StatusLegend?.Name != StatusLegendEnum.Cancelled.ToString()).Select(p => new IncidentLocationMapViewModel
                {
                    lat = p.Lat,
                    lon = p.Lng,
                    severity = p.SeverityLevel?.Name ?? string.Empty,
                    incidentStatus = p.StatusLegend?.Name ?? string.Empty,
                    color = p.StatusLegend?.Color ?? string.Empty,
                    incidentloc = p.LocationAddress ?? string.Empty,
                    calleraddress = p.CallerAddress ?? string.Empty,
                    callername = p.CallerName ?? string.Empty,
                    callerphone = p.CallerPhoneNumber ?? string.Empty,
                    incidentid = p.IncidentID,
                    assettype = GetAssets(assetIncidents, p.AssetIds ?? string.Empty),
                    description = p.DescriptionIssue ?? string.Empty,
                    eventtype = GetEventTypes(eventTypes, p.EventTypeIds ?? string.Empty),
                    intersection = p.Landmark ?? string.Empty,
                    perimeter = incidentValidations.Count > 0 ? GetPerimeter(incidentValidations.Where(i => i.IncidentId == p.Id).FirstOrDefault()?.DiscoveryPerimeterId) : ""
                }).ToList();


                // Define static colors for events
                var chartColors = new Dictionary<int, string>
                                {
                                    { 1, "#FF0000" }, { 2, "#00FF00" }, { 3, "#0000FF" },
                                    { 4, "#FFA500" }, { 5, "#800080" }, { 6, "#008080" },
                                    { 7, "#808000" }, { 10, "#FFD700" }, { 11, "#A52A2A" }
                                };

                // 🔹 Severity grouping
                var severityData = incidents
                    .GroupBy(i => i.SeverityLevelId)
                    .Select(g => new IncidentDashboardSeverityReportViewModel
                    {
                        color = severityLevels.FirstOrDefault(s => s.Id == g.Key)?.Color ?? "#808080",
                        name = severityLevels.FirstOrDefault(s => s.Id == g.Key)?.Name ?? "Unknown",
                        count = g.Count()
                    }).OrderByDescending(p => p.count).ToList();

                // 🔹 Status grouping
                var statusData = incidents
                    .GroupBy(i => i.StatusLegendId)
                    .Select(g => new IncidentDashboardStatusReportViewModel
                    {
                        color = statusLegends.FirstOrDefault(s => s.Id == g.Key)?.Color ?? "#808080",
                        name = statusLegends.FirstOrDefault(s => s.Id == g.Key)?.Name ?? "Unknown",
                        count = g.Count()
                    }).OrderByDescending(p => p.count).ToList();

                var eventOtherData = incidents
                        .SelectMany(i =>
                        {
                            // normal event type ids
                            var ids = (i.EventTypeIds ?? "")
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(id => int.Parse(id))
                                .ToList();

                            // if it's "Other", add a synthetic id (e.g. -1)
                            if (i.IsOtherEvent)
                            {
                                ids.Add(-1);
                            }

                            return ids.Select(id => new { EventId = id, Incident = i });
                        })
                        .GroupBy(x => x.EventId)
                        .Select(g => new
                        {
                            Id = g.Key,
                            Count = g.Count()
                        })
                        .ToList();

                // join with EventTypes + add "Other"
                var eventData = eventOtherData
                    .GroupJoin(_db.EventTypes,
                        g => g.Id,
                        e => e.Id,
                        (g, evts) => new { g, evts })
                    .SelectMany(
                        x => x.evts.DefaultIfEmpty(),
                        (x, e) => new IncidentDashboardEventTypeReportViewModel
                        {
                            color = x.g.Id == -1
                                ? "#000000" // color for "Other"
                                : (chartColors.ContainsKey(x.g.Id) ? chartColors[x.g.Id] : "#808080"),
                            name = x.g.Id == -1 ? "Other" : e.Name,
                            count = x.g.Count
                        }).OrderByDescending(p => p.count).ToList();


                // 🔹 Asset grouping
                var assetTypeData = incidents
                    .SelectMany(i => (i.AssetIds ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => int.Parse(id)))
                    .GroupBy(id => id)
                    .Select(g => new IncidentDashboardAssetTypeReportViewModel
                    {
                        name = assetIncidents.FirstOrDefault(a => a.Id == g.Key)?.Name ?? $"Asset {g.Key}",
                        count = g.Count(),
                        color = chartColors.ContainsKey(g.Key) ? chartColors[g.Key] : "#808080"
                    }).OrderByDescending(p => p.count).ToList();


                // Totals
                var totalIncidentCount = incidents.Count;
                var totalSeverity = severityData.Sum(x => x.count);
                var totalStatus = statusData.Sum(x => x.count);
                var totalEvent = eventData.Sum(x => x.count);
                var totalAssetType = assetTypeData.Sum(x => x.count);

                // Add percentages with a helper
                void AddPercentages<T>(IEnumerable<T> data, long total, Action<T, decimal> setPercent)
                {
                    foreach (var item in data)
                    {
                        var countProp = (long)item.GetType().GetProperty("count")?.GetValue(item)!;
                        var percent = total == 0 ? 0 : Math.Round((decimal)countProp / (long)total * 100, 2);
                        setPercent(item, percent);
                    }
                }

                AddPercentages(severityData, totalSeverity, (s, p) => ((IncidentDashboardSeverityReportViewModel)(object)s).SeverityPercentage = p);
                AddPercentages(statusData, totalStatus, (s, p) => ((IncidentDashboardStatusReportViewModel)(object)s).StatusPercentage = p);
                AddPercentages(eventData, totalEvent, (s, p) => ((IncidentDashboardEventTypeReportViewModel)(object)s).EventTypePercentage = p);
                AddPercentages(assetTypeData, totalAssetType, (s, p) => ((IncidentDashboardAssetTypeReportViewModel)(object)s).AssetTypePercentage = p);

                return new DashboardViewModel
                {
                    IncidentDashboard = new IncidentDashboardViewModel
                    {
                        SeverityLabels = severityData.Select(s => s.name).ToList(),
                        SeverityCounts = severityData.Select(s => s.count).ToList(),
                        SeverityColors = severityData.Select(s => s.color).ToList(),
                        StatusLabels = statusData.Select(s => s.name).ToList(),
                        StatusCounts = statusData.Select(s => s.count).ToList(),
                        StatusColors = statusData.Select(s => s.color).ToList(),
                        ListIncidentDashboardSeverityReportViewModel = severityData,
                        ListIncidentDashboardStatusReport = statusData,
                        ListIncidentDashboardEventTypeReportViewModel = eventData,
                        ListIncidentDashboardAssetTypeReportViewModel = assetTypeData,
                        TotalIncidentCount = totalIncidentCount,
                        TotalSeverityCount = totalSeverity,
                        TotalStatusLegendCount = totalStatus,
                        TotalEventTypeCount = totalEvent,
                        TotalAssetTypeCount = totalAssetType,
                        ListIncidentLocationMapViewModel = incidentLocation,
                        TotalSubmittedCount = incidents.Count(i => i.StatusLegend != null && i.StatusLegend.Name == StatusLegendEnum.Submitted.ToString()),
                        TotalValidatedCount = incidents.Count(i => i.StatusLegend != null && i.StatusLegend.Name == StatusLegendEnum.Validated.ToString()),
                        //TotalDispatchedCount = incidents.Count(i => i.StatusLegend.Name == StatusLegendEnum.Dispatched.ToString()),
                        TotalCompletedCount = incidents.Count(i => i.StatusLegend != null && i.StatusLegend.Name == StatusLegendEnum.Completed.ToString()),
                        TotalCancelledCount = incidents.Count(i => i.StatusLegend != null && i.StatusLegend.Name == StatusLegendEnum.Cancelled.ToString()),
                        ListIncidentRecentViewModel = incidentRecents
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetIncidentDashboardReport.");
                return new DashboardViewModel();
            }
        }

        private string GetEventTypes(List<EventType> eventTypes, string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return string.Empty;

            var idArray = ids.Split(",", StringSplitOptions.RemoveEmptyEntries)
                             .Select(id => long.TryParse(id.Trim(), out var val) ? val : (long?)null)
                             .Where(val => val.HasValue)
                             .Select(val => val.Value)
                             .ToList();

            var eventTypesName = eventTypes
                                      .Where(a => idArray.Contains(a.Id))
                                      .Select(a => a.Name)
                                      .ToList();

            return string.Join(",", eventTypesName);
        }

        private string GetAssets(List<AssetIncident> assetIncidents, string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return string.Empty;

            var idArray = ids.Split(",", StringSplitOptions.RemoveEmptyEntries)
                             .Select(id => long.TryParse(id.Trim(), out var val) ? val : (long?)null)
                             .Where(val => val.HasValue)
                             .Select(val => val.Value)
                             .ToList();

            var assetNames = assetIncidents
                                      .Where(a => idArray.Contains(a.Id))
                                      .Select(a => a.Name)
                                      .ToList();

            return string.Join(",", assetNames);
        }

        private string GetPerimeter(long? value) =>
           value switch
           {
               1 => "1 Mile",
               2 => "3 Miles",
               3 => "5 Miles",
               _ => string.Empty
           };
    }
}
