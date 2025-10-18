using System.ComponentModel.DataAnnotations;
using Models.Common.Interfaces;
using ViewModels.Shared;
using Enums;
using ViewModels;

namespace ViewModels
{
    public class StreetServiceRequestModifyViewModel : BaseUpdateVM, IBaseCrudViewModel, IIdentitifier
    {
        [Display(Name = "Subject", Prompt = "Subject")]
        public string Subject { get; set; }
        [Display(Name = "Name", Prompt = "Name")]
        public string Name { get; set; }
        [Phone]
        [Display(Name = "Phone", Prompt = "Phone")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Street Address", Prompt = "Street Address")]
        public string StreetAddress { get; set; }
        [Display(Name = "City", Prompt = "City")]
        public string City { get; set; }
        [Display(Name = "State", Prompt = "State")]
        public string State { get; set; }
        [Display(Name = "Zip Code", Prompt = "Zip Code")]
        public string? Zip { get; set; }
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Email")]
        public string Email { get; set; }
        [Display(Name = "Please call me")]
        public bool CallMe { get; set; }
        [Display(Name = "Please e-mail me")]
        public bool EmailMe { get; set; }
        [Display(Name = "Please handle, no need to contact me")]
        public bool NoNeedToContact { get; set; }
        [Display(Name = "Sidewalk", Prompt = "Sidewalk")]
        public bool SideWalk { get; set; }
        [Display(Name = "Potholes/Pavement", Prompt = "Potholes/Pavement")]
        public bool Potholes { get; set; }
        [Display(Name = "Drainage", Prompt = "Drainage")]
        public bool Drainage { get; set; }
        [Display(Name = "Street Sweeping", Prompt = "Street Sweeping")]
        public bool StreetSweeping { get; set; }
        [Display(Name = "Parkway Tree", Prompt = "Parkway Tree")]
        public bool ParkwayTree { get; set; }
        [Display(Name = "Other", Prompt = "Other")]
        public bool Other { get; set; }
        public string? Description { get; set; }


        [Display(Name = "Location of problem (closest street address)", Prompt = "Location of problem")]
        public string? LocationOfProblem { get; set; }
        [Display(Name = "Please use the box below to describe the problem", Prompt = "Problem Description")]
        public string? DescriptionOfProblem { get; set; }
        public SSRStatus? Status { get; set; } = SSRStatus.Open;
    }
}
