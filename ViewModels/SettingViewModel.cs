using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SettingViewModel
    {
        public RelationshipModifyViewModel Relationship { get; set; } = new();
        public List<RelationshipModifyViewModel> RelationshipList { get; set; } = new ();
    }
}
