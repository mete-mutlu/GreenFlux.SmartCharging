using GreenFlux.SmartCharging.API.Domain;

namespace GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;

public class ChargeStation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }

    public ICollection<Connector> Connectors { get; set; } = new List<Connector>();
}
