using GreenFlux.SmartCharging.API.Models.Request;
using System.ComponentModel.DataAnnotations;

namespace GreenFlux.SmartCharging.API.ModelValidation
{
    public class NoDuplicatesAttribute : ValidationAttribute
    {

        public override bool IsValid(object? value)
        {
            if (value is null)
                return true;
            HashSet<Connector> set = new();
            foreach (var element in (IEnumerable<Connector>)value)
                if (!set.Add(element))
                    return false;

            return true;
        }
    }
}
