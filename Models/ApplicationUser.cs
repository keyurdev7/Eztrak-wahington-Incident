using Enums;
using Microsoft.AspNetCore.Identity;
using Models.Common.Interfaces;
using Models.Models.Shared;

namespace Models;

// Add profile data for application users by adding properties to the ExampleUser class
public class ApplicationUser : IdentityUser<long>, IBaseModel
{
    public string? ImageUrl { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsApproved { get; set; }
    public ActiveStatus ActiveStatus { get; set; }
    public DateTime CreatedOn { get; set; }
    public long CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public DateTime LastLogin { get; set; }
    public long UpdatedBy { get; set; }

    public bool ChangePassword { get; set; }
    public string? PinCode { get; set; }
    public string? Department { get; set; }
}

