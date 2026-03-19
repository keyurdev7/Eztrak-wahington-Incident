
using Models.Models.Shared;
namespace Models
{
    public class EventType : BaseDBModel
    {
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }
}
