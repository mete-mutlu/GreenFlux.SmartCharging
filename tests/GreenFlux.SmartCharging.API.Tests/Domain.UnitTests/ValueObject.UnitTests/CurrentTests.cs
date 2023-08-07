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
    public class CurrentTests
    {
        [Fact]
        public void ShouldStoreValue()
        {
            var validValue = 50;
            var sut = new Current(validValue);
            sut.Value.Should().Be(validValue);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void ThrowsExceptionWhenValueIsInvalid(int invalidValue) => Assert.Throws<CurrentInvalidValueException>(() => new Current(invalidValue));
    }
}
