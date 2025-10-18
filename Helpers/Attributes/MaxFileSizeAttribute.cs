using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Helpers.Attributes
{

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInBytes;
        private readonly string _errorMessage;

        public MaxFileSizeAttribute(int maxFileSizeInBytes, string errorMessage = "")
        {
            _maxFileSizeInBytes = maxFileSizeInBytes;
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSizeInBytes)
                {
                    return new ValidationResult(!string.IsNullOrEmpty(_errorMessage) ? _errorMessage : $"The file size cannot exceed {_maxFileSizeInBytes / (1024 * 1024)} MB.");
                }
            }

            return ValidationResult.Success;
        }

    }
}

