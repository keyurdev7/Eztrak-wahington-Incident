using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Policy
{
    public class AddPolicyStepsRequest
    {
        public long PolicyId { get; set; }
        public List<string> Steps { get; set; } = new();
    }
}
