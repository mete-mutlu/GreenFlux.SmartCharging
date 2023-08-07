namespace GreenFlux.SmartCharging.API.Models.Response;

public record ChargeStationResponse(Guid Id, string Name, IEnumerable<ConnectorResponse> Connectors);

