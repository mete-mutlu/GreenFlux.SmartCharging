namespace GreenFlux.SmartCharging.API.Domain.Exceptions
{
    [Serializable]
    public class ConnectorIdOutOfRangeException : DomainException
    {
        public ConnectorIdOutOfRangeException(int minimum,int maximum) : base($"Id of a Connector can be between {minimum} and {maximum}.") { }

    }
}
