using Enums;
using System.ComponentModel;

namespace ViewModels.Shared
{
    public class BaseUpdateVM : BaseCrudViewModel
    {
        public long Id { get; set; }

        public bool IsCreated { get => Id <= 0; }
    }
}
