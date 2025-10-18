using DataLibrary;
using ViewModels.Incident;
using Microsoft.EntityFrameworkCore;
public class IncidentNoteService : IIncidentNoteService
{
    private readonly ApplicationDbContext _db;

    public IncidentNoteService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<IncidentNoteViewModel>> GetNotesByIncidentId(long incidentId)
    {
        return await _db.IncidentNotes
            .Where(n => n.IncidentId == incidentId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedOn)
            .Select(n => new IncidentNoteViewModel
            {
                Id = n.Id,
                Author = n.Author,
                NoteType = n.NoteType,
                Content = n.Content,
                FileUrl = n.FileUrl,
                CreatedOn = n.CreatedOn
            })
            .ToListAsync();
    }
    public async Task<IncidentNotesModalViewModel?> GetIncidentNotesModal(long incidentId)
    {
        var incident = await _db.Incidents
            .Include(i => i.StatusLegend)
            .Include(i => i.SeverityLevel)
            .FirstOrDefaultAsync(i => i.Id == incidentId);

        if (incident == null) return null;

        var notes = await GetNotesByIncidentId(incidentId);

        var viewModel = new IncidentNotesModalViewModel
        {
            IncidentId = incidentId,
            IncidentNumber = incident.IncidentID,
            Notes = notes,
            Status = incident.StatusLegend?.Name ?? "Unknown",
            Severity = incident.SeverityLevel?.Name ?? "Unknown",
            IncidentType = await GetEventTypes(incident.EventTypeIds ?? string.Empty),
            Location = incident.LocationAddress ?? string.Empty
        };

        return viewModel;
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

    public async Task AddNote(long incidentId, string author, string noteType, string content, string? fileUrl)
    {
        var note = new IncidentNote
        {
            IncidentId = incidentId,
            Author = author,
            NoteType = noteType,
            Content = content,
            FileUrl = fileUrl,
            CreatedOn = DateTime.Now,
            UpdatedOn = DateTime.Now
        };

        _db.IncidentNotes.Add(note);
        await _db.SaveChangesAsync();
    }
}
