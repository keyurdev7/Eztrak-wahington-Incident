using Models;
using Models.Models.Shared;

public class IncidentNote : BaseDBModel
{
    public long IncidentId { get; set; }
    public Incident Incident { get; set; }

    public string Author { get; set; } = string.Empty;
    public string NoteType { get; set; } = string.Empty; 
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
}
