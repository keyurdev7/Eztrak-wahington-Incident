using Models.Common.Interfaces;
using ViewModels.CRUD.Interfaces;
using ViewModels.Shared;

namespace ViewModels.CRUD
{
    public class CrudUpdateViewModel : ITitleViewModel
    {
        public CrudUpdateViewModel()
        {
            // FormType = "application/x-www-form-urlencoded";
            FormType = "multipart/form-data";
            FormAction = "Update";
            UpdateViewPath = "_Update";
        }
        public string Title { get; set; }
        public string Name { get; set; }
        public string FormId { get; set; }
        public string FormType { get; set; }
        public string FormAction { get; set; }
        public string FormController { get; set; }
        public string UpdateViewPath { get; set; }
        public IBaseCrudViewModel UpdateModel { get; set; }
        public bool IsApprovalForm { get; set; }
        public bool IsAjaxBased { get; set; } = true;
        public string SubmitOnClick { get; set; } = "updateRecord(this)";
        public bool IsUnAuthorized { get; set; } = true;
    }
}
