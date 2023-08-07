using GreenFlux.SmartCharging.API.Domain.Exceptions;
using System.IO;

namespace GreenFlux.SmartCharging.API.Domain.ValueObjects
{
    public class Current : ValueObject
    {

        public int Value { get; private set; }


        public Current(int value)
        {
            if (value < 1)
                throw new CurrentInvalidValueException();
            Value = value;
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Current(int value) => new Current(value);

    }
}
