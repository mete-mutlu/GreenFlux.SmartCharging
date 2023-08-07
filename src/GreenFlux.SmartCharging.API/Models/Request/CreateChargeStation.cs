using System.ComponentModel.DataAnnotations;

namespace GreenFlux.SmartCharging.API.Models.Request
{
    public record CreateChargeStation
    {
        [MinLength(1)]
        public required string Name { get; set; }
        public required IEnumerable<Connector> Connectors { get; set; }
    }
}
