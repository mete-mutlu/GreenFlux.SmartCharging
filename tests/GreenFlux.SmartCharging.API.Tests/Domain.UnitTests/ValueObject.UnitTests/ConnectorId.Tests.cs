using FluentAssertions;
using GreenFlux.SmartCharging.API.Domain.Exceptions;
using GreenFlux.SmartCharging.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenFlux.SmartCharging.API.Tests.Domain.UnitTests.ValueObject.UnitTests
{
    public class ConnectorIdTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void ShouldStoreValue(int validValue)
        {
            var sut = new ConnectorId(validValue);
            sut.Value.Should().Be(validValue);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(0)]
        public void ThrowsExceptionWhenValueIsInvalid(int invalidValue) => Assert.Throws<ConnectorIdOutOfRangeException>(() => new ConnectorId(invalidValue));
    }
}
