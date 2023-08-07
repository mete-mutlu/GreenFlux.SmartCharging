using GreenFlux.SmartCharging.API.ModelValidation;
using System.ComponentModel.DataAnnotations;

namespace GreenFlux.SmartCharging.API.Models.Request
{
    public record CreateChargeStation
    {
        [MinLength(1)]
        public required string Name { get; set; }
        [NoDuplicates]
        public required IEnumerable<Connector> Connectors { get; set; }
    }
}
