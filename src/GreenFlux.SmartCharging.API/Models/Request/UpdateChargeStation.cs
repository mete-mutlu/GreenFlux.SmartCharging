using System.ComponentModel.DataAnnotations;

namespace GreenFlux.SmartCharging.API.Models.Request
{
    public record UpdateChargeStation
    {
        [MinLength(1)]
        public required string Name { get; set; }

    }
}
