using Models;

namespace ViewModels.Incident
{
    public class TeamWithUsersViewModel
    {
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public List<IncidentUser> Users { get; set; } = new();
    }
}