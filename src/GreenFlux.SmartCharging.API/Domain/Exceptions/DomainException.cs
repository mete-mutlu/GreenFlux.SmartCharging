namespace GreenFlux.SmartCharging.API.Domain.Exceptions
{
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }

    }
}
