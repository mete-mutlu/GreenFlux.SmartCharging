namespace GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;

public class Group
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public ICollection<ChargeStation> ChargeStations { get; set; } = new List<ChargeStation>();
}

