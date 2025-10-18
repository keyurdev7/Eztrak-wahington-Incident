using Models.Models.Shared;

namespace Models
{
    public class CraftSkill : BaseDBModel
    {
        public string Name { get; set; }
        public double STRate { get; set; }
        public double OTRate { get; set; }
        public double DTRate { get; set; }
    }
}
