using Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Attributes
{
    public class DateComparisonAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _comparisonProperty;
        private readonly ComparisonCatalog _comparison;

        public DateComparisonAttribute(string comparisonProperty, ComparisonCatalog comparison = ComparisonCatalog.EqualTo)
        {
            _comparisonProperty = comparisonProperty;
            _comparison = comparison;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            switch (_comparison)
            {
                case ComparisonCatalog.EqualTo:
                    if (currentValue != comparisonValue)
                        return new ValidationResult(ErrorMessage);
                    break;
                case ComparisonCatalog.GreaterThan:
                    if (currentValue < comparisonValue)
                        return new ValidationResult(ErrorMessage);
                    break;
                case ComparisonCatalog.LessThan:
                    if (currentValue > comparisonValue)
                        return new ValidationResult(ErrorMessage);
                    break;
            }

            return ValidationResult.Success;
        }
        public void AddValidation(ClientModelValidationContext context)
        {
            var error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-error", error);
        }
    }
}
