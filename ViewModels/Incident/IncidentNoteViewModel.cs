namespace ViewModels.Incident
{
    public class IncidentNoteViewModel
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }

        public string Author { get; set; } = string.Empty;
        public string NoteType { get; set; } = string.Empty; // Internal / External
        public string Content { get; set; } = string.Empty;
        public string? FileUrl { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
