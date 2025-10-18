using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Shared
{
    public class ResponseModel<T> where T : new()
    {
        public int Status { get; set; }
        public T Data { get; set; }
    }
}
