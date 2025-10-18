using DataLibrary;
using Repositories.Services.IncidentHistory.Interface;
using ViewModels.Incident;


using Microsoft.EntityFrameworkCore;

public class IncidentHistoryService : IIncidentHistoryService
{
    private readonly ApplicationDbContext _db;

    public IncidentHistoryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IncidentHistoryViewModel> GetIncidentHistoryAsync(long incidentId)
    {
        var incident = await _db.Incidents
            .Include(i => i.SeverityLevel)
            .FirstOrDefaultAsync(i => i.Id == incidentId);

        if (incident == null) return new IncidentHistoryViewModel();

        var histories = await _db.IncidentHistories
            .Include(h => h.StatusLegend)
            .Where(h => h.IncidentId == incidentId)
            .OrderBy(h => h.CreatedOn)
            .ToListAsync();

        var notesCount = await _db.IncidentNotes.CountAsync(n => n.IncidentId == incidentId);
        var eventTypes = await GetEventTypes(incident.EventTypeIds ?? string.Empty);
        var vm = new IncidentHistoryViewModel
        {
            IncidentId = incident.Id,
            IncidentNumber = incident.IncidentID,
            StatusChangeCount = histories.Count,
            NotesCount = notesCount,
            CreatedOn = incident.CreatedOn,
            UpdatedOn = incident.UpdatedOn,
            HistoryRecords = histories.Select(h => new IncidentHistoryRecord
            {
                StatusName = h.StatusLegend.Name,
                StatusColor = h.StatusLegend.Color,
                Location = incident.LocationAddress ?? "-",
                Intersection = incident.Landmark ?? "-",
                EventType = eventTypes,
                Severity = incident.SeverityLevel?.Name ?? "-",
                SeverityColor = incident.SeverityLevel?.Color ?? string.Empty,
                Description = incident.DescriptionIssue ?? "-",
                EsIndicators = GetIndicator(incident.GasPresentId)
            }).ToList()
        };

        return vm;
    }
    public async Task<string> GetEventTypes(string ids)
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
    private string GetIndicator(long? value) =>
           value switch
           {
               1 => "Yes",
               0 => "No",
               2 => "N/A",
               _ => string.Empty
           };
}
