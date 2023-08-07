using GreenFlux.SmartCharging.API.Domain.Exceptions;
using GreenFlux.SmartCharging.API.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace GreenFlux.SmartCharging.API.Domain
{
    public class ChargeStation
    {
        private readonly int maxSupportedConnectorsCount = 5;

        public Guid Id { get;  init; }

        public string Name { get; private set; }

        internal List<Connector> _connectors = new();

        public ReadOnlyCollection<Connector> Connectors => new(_connectors);


        internal ChargeStation(Guid id, string name, IEnumerable<Connector> connectors)
        {
            ValidateAndAdd(connectors);
            Name = name;
            Id = id;
        }

        internal ChargeStation(string name, IEnumerable<Connector> connectors)
        {
            ValidateAndAdd(connectors);
            Name = name;
        }

        internal void Update(string name) => Name = name;

        internal void AddConnectors(IEnumerable<Connector> connectors) => ValidateAndAdd(connectors);

        internal void UpdateConnectors(IEnumerable<Connector> connectors)
        {
            foreach (var connector in connectors)
            {
                var index = _connectors.FindIndex(p => p.Id == connector.Id);
                if (index != -1)
                    _connectors[index].Update(connector.MaxCurrent);
            }
        }


        internal void RemoveConnectors(IEnumerable<ConnectorId> ids)
        {
            if (ids.Count() >= _connectors.Count)
                throw new ConnectorCountException(maxSupportedConnectorsCount);

            foreach (var connector in _connectors.Where(c=> ids.Contains(c.Id)).ToList())
                _connectors.Remove(connector);
        }

        private void ValidateAndAdd(IEnumerable<Connector> connectors)
        {
            if (_connectors.Count + connectors.Count() > maxSupportedConnectorsCount)
                throw new ConnectorCountException(maxSupportedConnectorsCount);

            _connectors.AddRange(connectors);
        }

    }
}
