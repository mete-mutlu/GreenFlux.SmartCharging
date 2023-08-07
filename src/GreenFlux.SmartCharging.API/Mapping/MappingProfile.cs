using AutoMapper;
using GreenFlux.SmartCharging.API.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using Entities = GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using Request = GreenFlux.SmartCharging.API.Models.Request;
using Response = GreenFlux.SmartCharging.API.Models.Response;
namespace GreenFlux.SmartCharging.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Entities.Group, Domain.Group>()
              .ForMember(dest => dest.ChargeStations, opt => opt.Ignore())
              .ConstructUsing(e => new Domain.Group(e.Id, e.Name, e.Capacity, e.ChargeStations.Select(cs => new Tuple<Guid, string, IEnumerable<(int, int)>>(cs.Id, cs.Name, cs.Connectors.Select(c => new Tuple<int, int>(c.Id, c.MaxCurrent).ToValueTuple())).ToValueTuple())));

            CreateMap<Domain.Group, Entities.Group>()
               .ForMember(dest => dest.ChargeStations, opt => opt.Ignore())
               .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity.Value));

            CreateMap<Domain.ChargeStation, Entities.ChargeStation>()
              .ForMember(dest => dest.Group, opt => opt.Ignore())
              .ForMember(dest => dest.GroupId, opt => opt.Ignore());

            CreateMap<Domain.Connector, Entities.Connector>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
                  .ForMember(dest => dest.MaxCurrent, opt => opt.MapFrom(src => src.MaxCurrent.Value))
                  .ForMember(dest => dest.ChargeStation, opt => opt.Ignore())
                  .ForMember(dest => dest.ChargeStationId, opt => opt.Ignore());

            CreateMap<Entities.Connector, Domain.Connector>()
                  .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new ConnectorId(src.Id)))
                  .ForMember(dest => dest.MaxCurrent, opt => opt.MapFrom(src => src.MaxCurrent));

            CreateMap<Entities.Group, Response.GroupResponse>();
            CreateMap<Entities.ChargeStation, Response.ChargeStationResponse>().ForSourceMember(m=>m.Group, opt=>opt.DoNotValidate());
            CreateMap<Entities.Connector, Response.ConnectorResponse>().ForSourceMember(m => m.ChargeStation, opt => opt.DoNotValidate());
        }
    }
}
