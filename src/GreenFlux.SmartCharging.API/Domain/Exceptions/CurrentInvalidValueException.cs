namespace GreenFlux.SmartCharging.API.Domain.Exceptions
{
    [Serializable]
    public class CurrentInvalidValueException : DomainException
    {
        public CurrentInvalidValueException() : base($"Current must be a positive value") { }

    }
}
