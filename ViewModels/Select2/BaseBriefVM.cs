using Helper.Attributes;
using System.ComponentModel;

namespace ViewModels
{
    public interface IBaseBriefVM
    {
        long Id { get; set; }
        string Name { get; set; }
    }
    public class BaseBriefVM : IBaseBriefVM
    {
        public string ErrorMessage { get; } = "";
        public bool IsValidationEnabled { get; }

        public BaseBriefVM()
        {
            IsValidationEnabled = false;
            ErrorMessage = "";
        }
        public BaseBriefVM(bool isValidationEnabled)
        {
            IsValidationEnabled = isValidationEnabled;
            ErrorMessage = "";
        }
        public BaseBriefVM(bool isValidationEnabled, string errorMessage)
        {
            IsValidationEnabled = isValidationEnabled;
            ErrorMessage = errorMessage;
        }
        [RequiredSelect2("ErrorMessage", "IsValidationEnabled")]
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
    }

    public interface ISelect2BaseVM
    {
        long? Id { get; set; }
        string? Select2Text { get; }
    }
    public class BaseSelect2VM : ISelect2BaseVM
    {
        public string ErrorMessage { get; } = "";
        public bool IsValidationEnabled { get; }

        public BaseSelect2VM()
        {
            IsValidationEnabled = false;
            ErrorMessage = "";
        }
        public BaseSelect2VM(bool isValidationEnabled)
        {
            IsValidationEnabled = isValidationEnabled;
            ErrorMessage = "";
        }
        public BaseSelect2VM(bool isValidationEnabled, string errorMessage)
        {
            IsValidationEnabled = isValidationEnabled;
            ErrorMessage = errorMessage;
        }
        [RequiredSelect2("ErrorMessage", "IsValidationEnabled")]
        public virtual long? Id { get; set; }
        public virtual string? Select2Text { get; set; }
    }

    public class BaseImageBriefVM : BaseSelect2VM
    {
        public string ImageUrl { get; set; }
    }
}
