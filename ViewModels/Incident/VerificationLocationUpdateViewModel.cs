namespace ViewModels.Incident
{
    public class VerificationLocationUpdateViewModel
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }

        public string VerificationStatus { get; set; } = "Pending";
        public string? VerificationNotes { get; set; }

        public string? ServiceAccount { get; set; }
        public string? AssetIDs { get; set; } // comma-separated IDs
    }
}

