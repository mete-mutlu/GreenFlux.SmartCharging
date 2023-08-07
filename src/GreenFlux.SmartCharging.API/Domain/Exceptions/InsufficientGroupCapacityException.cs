using GreenFlux.SmartCharging.API.Domain.ValueObjects;

namespace GreenFlux.SmartCharging.API.Domain.Exceptions
{
    [Serializable]
    public class InsufficientGroupCapacityException : DomainException
    {
        public InsufficientGroupCapacityException(int groupCapacity, int currentInUse) : base($"Group Capacity is not enough for this operation. Group Capacity: {groupCapacity}, Current In Use:{currentInUse}") { }

    }
}
