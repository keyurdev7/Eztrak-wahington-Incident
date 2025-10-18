using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class MUTCD : BaseDBModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
