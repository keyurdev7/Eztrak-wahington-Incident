using System;
using System.Collections.Generic;
using System.Text;

namespace Select2.Model
{
    public class Select2ViewModel
    {
        public string id { get; set; }

        public string text { get; set; }
    }

    public class Select2PagedResultViewModel
    {
        public int Total { get; set; }

        public List<Select2ViewModel> Results { get; set; }
    }

    public class Select2OptionModel<T>
    {
        public string id { get; set; }

        public string text { get; set; }

        public T additionalAttributesModel { get; set; }
    }

    public class Select2PagedResult<T>
    {
        public int Total { get; set; }

        public List<Select2OptionModel<T>> Results { get; set; }
    }
}
