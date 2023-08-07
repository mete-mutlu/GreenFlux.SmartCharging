namespace GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;

public class Connector
{
    public int Id { get; set; }

    public int MaxCurrent { get; set; }

    public Guid ChargeStationId { get; set; }
    public ChargeStation ChargeStation { get; set; }
}

