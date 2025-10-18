using Enums;
using Models.Models.Shared;

namespace Models
{
    public class UserSearchSetting : BaseDBModel
    {
        public string Name { get; set; }
        public SearchFilterTypeCatalog Type { get; set; }
        public string FilterJson { get; set; }
        public string ClassName { get; set; }
        public string ViewPath { get; set; }
    }
}
