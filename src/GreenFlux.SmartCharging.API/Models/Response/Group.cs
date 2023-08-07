namespace GreenFlux.SmartCharging.API.Models.Response;

public record GroupResponse(string Id, string Name, int Capacity, IEnumerable<ChargeStationResponse> ChargeStations);

