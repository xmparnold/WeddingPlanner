using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models;

public class FutureDateAttribute : ValidationAttribute 
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        

        if (value == null) {
            return ValidationResult.Success;
        }
        DateTime date = (DateTime)value;
        if (date <= DateTime.Now)
        {
            return new ValidationResult("must be a future date");
        }
       
        return ValidationResult.Success;
    }
}