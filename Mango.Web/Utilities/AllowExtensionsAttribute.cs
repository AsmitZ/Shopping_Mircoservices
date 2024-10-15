using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utilities;

public class AllowExtensionsAttribute(string[] extensions) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!extensions.Contains(extension.ToLower()))
            {
                return new ValidationResult($"This image extension {extension} is not allowed!");
            }
        }

        return ValidationResult.Success;
    }
}