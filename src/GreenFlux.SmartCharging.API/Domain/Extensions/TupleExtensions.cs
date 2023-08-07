
using GreenFlux.SmartCharging.API.Domain.ValueObjects;

namespace GreenFlux.SmartCharging.API.Domain.Extensions
{
    public static class TupleExtensions
    {
        public static ChargeStation ToDomainObject(this (string Name, IEnumerable<(int Id, int MaxCurrent)> Connectors) model) => new(model.Name, model.Connectors.Select(c => new Connector(c.Id, c.MaxCurrent)));
    }
}
