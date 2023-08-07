using GreenFlux.SmartCharging.API.Domain.Exceptions;
using System.IO;

namespace GreenFlux.SmartCharging.API.Domain.ValueObjects
{
    public class ConnectorId : ValueObject
    {

        public int _minimumValue = 1;
        public int _maximumValue = 5;


        public int Value { get; }


        public ConnectorId(int value)
        {
            if (value < _minimumValue || value > _maximumValue)
                throw new ConnectorIdOutOfRangeException(_minimumValue, _maximumValue);
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }


        public static implicit operator ConnectorId(int value) => new ConnectorId(value);
    }
}
