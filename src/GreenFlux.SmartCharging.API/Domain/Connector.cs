using GreenFlux.SmartCharging.API.Domain.ValueObjects;

namespace GreenFlux.SmartCharging.API.Domain
{
    public class Connector
    {
        internal Connector(ConnectorId id, Current maxCurrent)
        {
            Id = id;
            MaxCurrent = maxCurrent;
        }

        internal void Update(Current maxCurrent) => MaxCurrent = maxCurrent;

        public ConnectorId Id { get; init; }

        public Current MaxCurrent { get; private set; }
       
    }
}
