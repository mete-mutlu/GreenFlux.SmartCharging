using System.ComponentModel.DataAnnotations;

namespace GreenFlux.SmartCharging.API.Models.Request
{
    public record Group
    {
        [MinLength(1)]
        public required string Name { get; set; }
        public required int Capacity { get; set; }
    }
}
