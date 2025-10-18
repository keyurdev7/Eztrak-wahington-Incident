namespace ViewModels.Incident
{
    public class IncidentNotesModalViewModel
    {
        public long IncidentId { get; set; }
        public List<IncidentNoteViewModel> Notes { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string IncidentType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string IncidentNumber { get; set; } = string.Empty;
    }
}

