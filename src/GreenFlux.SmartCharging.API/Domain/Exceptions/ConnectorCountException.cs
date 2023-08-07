namespace GreenFlux.SmartCharging.API.Domain.Exceptions
{
    [Serializable]
    public class ConnectorCountException : DomainException
    {
        public ConnectorCountException(int count) : base($"Operation is not allowed. A Charge Station must have at least 1, at most {count} connectors.") { }

    }
}
