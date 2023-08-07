using GreenFlux.SmartCharging.API.ModelValidation;

namespace GreenFlux.SmartCharging.API.Models.Request
{
    public record Connectors
    {
        [NoDuplicates]
        public required IEnumerable<Connector> List { get; set; }
    }
}
