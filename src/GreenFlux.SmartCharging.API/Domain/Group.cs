using GreenFlux.SmartCharging.API.Domain.Exceptions;
using GreenFlux.SmartCharging.API.Domain.Extensions;
using GreenFlux.SmartCharging.API.Domain.ValueObjects;
using GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using System.Collections.ObjectModel;

namespace GreenFlux.SmartCharging.API.Domain
{
    /// <summary>
    /// Aggregate Root
    /// </summary>
    public class Group
    {
        public Guid Id { get; init; }
        public string Name { get; private set; }
        public Current Capacity { get; private set; }

        private List<ChargeStation> _chargeStations = new();

        public ReadOnlyCollection<ChargeStation> ChargeStations => new(_chargeStations);

        public Group(string name, int capacity)
        {
            Name = name;
            Capacity = capacity;
        }

        public Group(Guid id, string name, int capacity, IEnumerable<(Guid Id, string Name, IEnumerable<(int Id, int MaxCurrent)> Connectors)> chargeStations)
        {
            Id = id;
            Name = name;
            Capacity = capacity;
            _chargeStations = chargeStations.Select(cs => new ChargeStation(cs.Id, cs.Name, cs.Connectors.Select(c => new Connector(c.Id, c.MaxCurrent)))).ToList();
        }


        public void Update(string name, int capacity)
        {
            int currentInUse = GetCurrentInUse();
            if (capacity < currentInUse)
                throw new CapacityInUseException(currentInUse, _chargeStations.Select(p=>p.Name));
            Name = name;
            Capacity = capacity;
          
        }

        public void AddChargeStation(string name, IEnumerable<(int Id, int MaxCurrent)> connectors)
        {
            int currentInUse = GetCurrentInUse();
            if (Capacity.Value < currentInUse + connectors.Sum(p => p.MaxCurrent))
                throw new InsufficientGroupCapacityException(Capacity.Value, currentInUse);
            _chargeStations.Add((name, connectors).ToDomainObject());
        }

        public void UpdateChargeStation(Guid id,string name)
        {
            var index = _chargeStations.FindIndex(p => p.Id == id);
            _chargeStations[index].Update(name);
        }

        public void AddConnectors(Guid chargeStationId, IEnumerable<(int Id, int MaxCurrent)> connectors)
        {
            ChargeStation? chargeStation = FindChargeStation(chargeStationId);
            if (chargeStation is not null)
            {
                var currentInUse = GetCurrentInUse();
                if (Capacity.Value < currentInUse + connectors.Select(p => p.MaxCurrent).Sum())
                    throw new InsufficientGroupCapacityException(Capacity.Value, currentInUse);
                chargeStation.AddConnectors(connectors.Select(c => new Connector(c.Id, c.MaxCurrent)));
            }
        }

        public void RemoveConnectors(Guid chargeStationId, IEnumerable<int> ids)
        {
            ChargeStation chargeStation = FindChargeStation(chargeStationId)!;
            chargeStation.RemoveConnectors(ids.Select(p => new ConnectorId(p)));
        }

        public void UpdateConnectors(Guid chargeStationId, IEnumerable<(int Id, int MaxCurrent)> connectors)
        {
            var currentInUse = GetCurrentInUse();
            if (Capacity.Value < currentInUse + connectors.Sum(p => p.MaxCurrent) - GetCurrentInUseByConnectors(connectors.Select(p=>p.Id)))
                throw new InsufficientGroupCapacityException(Capacity.Value, currentInUse);
            ChargeStation chargeStation = FindChargeStation(chargeStationId)!;
            foreach (var connector in connectors)
                chargeStation.Connectors.Single(p => p.Id.Value == connector.Id).Update(connector.MaxCurrent);

        }

        private ChargeStation? FindChargeStation(Guid chargeStationId) => _chargeStations.Find(p => p.Id == chargeStationId);


        private int GetCurrentInUseByConnectors(IEnumerable<int> ids) => _chargeStations.Sum(cs => cs.Connectors.Where(p=> ids.Contains(p.Id.Value)).Sum(c => c.MaxCurrent.Value));
        private int GetCurrentInUse() => _chargeStations.Sum(cs => cs.Connectors.Sum(c => c.MaxCurrent.Value));

    }
}
