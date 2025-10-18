using System.ComponentModel.DataAnnotations.Schema;
using Enums;
using Models.Models.Shared;

namespace Models
{
    public class StreetServiceRequest : BaseDBModel
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string? Zip { get; set; }
        public string Email { get; set; }
        public bool CallMe { get; set; }
        public bool EmailMe { get; set; }
        public bool NoNeedToContact { get; set; }
        public bool SideWalk { get; set; }
        public bool Potholes { get; set; }
        public bool Drainage { get; set; }
        public bool StreetSweeping { get; set; }
        public bool ParkwayTree { get; set; }
        public bool Other { get; set; }
        public string? Description { get; set; }
        public string? LocationOfProblem { get; set; }
        public string? DescriptionOfProblem { get; set; }
        public SSRStatus? Status { get; set; }

        public WorkOrder? WorkOrder { get; set; }
    }
}
