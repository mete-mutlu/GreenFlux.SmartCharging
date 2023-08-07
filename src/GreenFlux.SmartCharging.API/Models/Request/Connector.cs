using GreenFlux.SmartCharging.API.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GreenFlux.SmartCharging.API.Models.Request
{
    public record Connector
    {
        [SetsRequiredMembers]
        public Connector(int id, int maxCurrent)
        {
            Id = id;
            MaxCurrent = maxCurrent;
        }

        public required int Id { get; set; }


        public required int MaxCurrent { get; set; }


    }
}
