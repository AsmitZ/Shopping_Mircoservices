using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utilities;

public class MaxFileSizeAttribute(int max) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
        {
            return ValidationResult.Success;
        }

        if (file.Length > max)
        {
            return new ValidationResult($"The file exceeds the {max} bytes limit.");
        }

        return ValidationResult.Success;
    }
}