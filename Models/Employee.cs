using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models.Models.Shared;

namespace Models
{
    public class Employee : BaseDBModel
    {

        [StringLength(1000)]
        public string? AddressLine1 { get; set; }

        [StringLength(1000)]
        public string? AddressLine2 { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Zip { get; set; }

        // Foreign key
        public long UserId { get; set; }
        // Navigation property for the one-to-one relationship
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

    }
}