using GreenFlux.SmartCharging.API.Domain.ValueObjects;

namespace GreenFlux.SmartCharging.API.Domain.Exceptions
{
    [Serializable]
    public class CapacityInUseException : DomainException
    {
        public CapacityInUseException(int capacityInUse,IEnumerable<string> chargeStationNames) : base($"Capacity cannot be updated. {capacityInUse} amps is already in allocated for {chargeStationNames}." +
            $"Consider allocate more capacity or removing those stations.") { }

    }
}
