using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Shared.Notes
{
    public interface IHasNotes
    {
        public bool HasNotes { get; set; }
    }
}
