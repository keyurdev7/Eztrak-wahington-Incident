using Select2.Model;
using System.ComponentModel;

namespace ViewModels
{
    public class OrderBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public override string? Select2Text { get => Id.ToString(); }
    }


}
