using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ChatDesktop.Attributes
{
    public class NotEqualValidationAttribute : ValidationAttribute
    {
        string propertyToCompare;
        public NotEqualValidationAttribute(string propertyToCompare)
        {
            this.propertyToCompare = propertyToCompare;
        }

        public NotEqualValidationAttribute(string propertyToCompare, string errorMessage) : this(propertyToCompare)
        {
            this.ErrorMessage = propertyToCompare;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propInfo = validationContext.ObjectInstance.GetType().GetProperty(propertyToCompare);
            if (propInfo != null)
            {
                var propValue = propInfo.GetValue(validationContext.ObjectInstance);
                IEnumerable<string> compareElement = new List<string> { validationContext.MemberName };
                if (value != null && propValue != null && !string.IsNullOrEmpty(value.ToString()) && !string.IsNullOrEmpty(propValue.ToString()) //if either one is empty dont Validate
                    && (value.ToString() == propValue.ToString()))
                    return new ValidationResult(ErrorMessage, compareElement);
            }
            else
                throw new NullReferenceException("propertyToCompare must be the name of property to compare");

            return ValidationResult.Success;
        }
    }
}
