using GreenFlux.SmartCharging.API.Models.Request;
using System.ComponentModel.DataAnnotations;

namespace GreenFlux.SmartCharging.API.ModelValidation
{
    public class NoDuplicatesAttribute : ValidationAttribute
    {
        
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            HashSet<Connector> set = new();
            foreach (var element in (IEnumerable<Connector>)value)
                if (!set.Add(element))
                    return new ValidationResult("There are duplicates on connectors.");

            return ValidationResult.Success;
        }
    }
}
