using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Policy : BaseDBModel
    {
        [ForeignKey(nameof(TeamId))]
        public long? TeamId { get; set; }

        public IncidentTeam Team { get; set; }
        public string Name { get; set; }
        public string? TeamIds { get; set; }
        public string Description { get; set; }
        public string? PolicySteps { get; set; }


    }
}
