using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GreenFlux.SmartCharging.API.Tests.API.IntegrationTests
{

    [Collection("IntegrationTestsCollection")]
    public class TestHostFixture : ICollectionFixture<WebApplicationFactory<Program>>
    {
        public readonly HttpClient Client;
        public readonly WebApplicationFactory<Program> Factory;

        public TestHostFixture()
        {
            Factory = new WebApplicationFactory<Program>();
            Client = Factory.CreateClient();
        }
    }
}
