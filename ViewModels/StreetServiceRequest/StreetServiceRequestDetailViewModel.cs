using Enums;
using Helpers.Datetime;
using Helpers.Extensions;
using System.ComponentModel.DataAnnotations;
using ViewModels.Shared;

namespace ViewModels
{
    public class StreetServiceRequestDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string? Subject { get; set; }
        public string Name { get; set; }

        public bool HasNotes { get; set; }

        public string HasNotesClass
        {
            get
            {
                return HasNotes ? "has-note" : "";
            }
        }
        public DateTime CreatedOn { get; set; }

        public string FormattedCreatedOn
        {
            get
            {
                return CreatedOn.FormatDate();
            }
        }
        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string? Zip { get; set; }
        public string Email { get; set; }
        [Display(Name = "Please call me")]
        public bool CallMe { get; set; }
        [Display(Name = "Please e-mail me")]
        public bool EmailMe { get; set; }
        [Display(Name = "Please handle, no need to contact me")]
        public bool NoNeedToContact { get; set; }
        [Display(Name = "Sidewalk")]
        public bool SideWalk { get; set; }
        [Display(Name = "Potholes/Pavement")]
        public bool Potholes { get; set; }
        [Display(Name = "Drainage")]
        public bool Drainage { get; set; }
        [Display(Name = "Street Sweeping")]
        public bool StreetSweeping { get; set; }
        [Display(Name = "Parkway Tree")]
        public bool ParkwayTree { get; set; }
        [Display(Name = "Other")]
        public bool Other { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Location of problem (closest street address)")]
        public string? LocationOfProblem { get; set; }
        [Display(Name = "Please use the box below to describe the problem")]
        public string? DescriptionOfProblem { get; set; }
        public SSRStatus? Status { get; set; }
    }
}
