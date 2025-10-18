using Models.Models.Shared;

namespace Models
{
    public class SeverityLevel : BaseDBModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
    }
}
