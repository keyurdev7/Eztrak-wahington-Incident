using ViewModels.CRUD.Interfaces;
using ViewModels.Shared;

namespace ViewModels.CRUD
{
    public class CrudDetailViewModel : ITitleViewModel
    {
        public CrudDetailViewModel()
        {
            DetailViewPath = "_Detail";
        }
        public string Title { get; set; }
        public string DetailViewPath { get; set; }
        public bool IsApprovalForm { get; set; }
        public IBaseCrudViewModel DetailModel { get; set; }
    }
}
