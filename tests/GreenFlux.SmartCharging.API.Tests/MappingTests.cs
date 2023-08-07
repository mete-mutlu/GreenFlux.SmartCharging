using AutoMapper;
using GreenFlux.SmartCharging.API.Mapping;
using Xunit;

namespace GreenFlux.SmartCharging.API.Tests;

public class MappingProfileTests
{
    [Fact]
    public void ValidateMappingConfigurationTest()
    {
        var mapperConfig = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });

        IMapper mapper = new Mapper(mapperConfig);

        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
