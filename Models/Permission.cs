using Models.Models.Shared;

namespace Models
{
    public class Permission : BaseDBModel
    {
        public string Name { get; set; }
        public string Screen { get; set; }
    }
}
