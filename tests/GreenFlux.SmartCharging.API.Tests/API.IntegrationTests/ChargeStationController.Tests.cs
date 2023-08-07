using FluentAssertions;
using GreenFlux.SmartCharging.API.Persistence.EntityFramework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using Entities = GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using GreenFlux.SmartCharging.API.Models.Request;
using Microsoft.AspNetCore.Hosting;

namespace GreenFlux.SmartCharging.API.Tests.API.IntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public class ChargeStationControllerTests
    {

        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _controllerBaseRoute = "api/chargeStation";

        public ChargeStationControllerTests(TestHostFixture fixture)
        {
            _client = fixture.Client;
            _factory = fixture.Factory;
        }

     
        private (Guid GroupId, Guid ChargeStationId) SeedData()
        {

            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();;
            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();
            var chargeStation = new Entities.ChargeStation { Name = "Charge Station", Connectors = new List<Entities.Connector> { new Entities.Connector { Id = 1, MaxCurrent = 10 } } };
            var group = new Entities.Group { Name = "Existing Group", Capacity = 1000, ChargeStations = new List<Entities.ChargeStation> { chargeStation } };
            dataContext.Groups.Add(group);
            dataContext.SaveChanges();
            return (group.Id, chargeStation.Id);
        }

        [Fact]
        public async Task Create_Should_Return_200OK()
        {
            // Arrange
            var (groupId, _) = SeedData();
            var model = new CreateChargeStation() { Name = "CS", Connectors = new List<Connector> { new Connector(2, 15) } };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{_controllerBaseRoute}/{groupId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var group = dataContext.Groups.Include(p => p.ChargeStations).ThenInclude(p => p.Connectors).Single();
            group.ChargeStations.Should().ContainEquivalentOf(model);

        }

        [Theory]
        public async Task Create_Should_Return_400BadRequest()
        {
            // Arrange
            var (groupId, _) = SeedData();
            var model = new CreateChargeStation() { Name = "CS", Connectors = new List<Connector> { new Connector(2, 15) } };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{_controllerBaseRoute}/{groupId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var group = dataContext.Groups.Include(p => p.ChargeStations).ThenInclude(p => p.Connectors).Single();
            group.ChargeStations.Should().ContainEquivalentOf(model);

        }



        [Fact]
        public async Task CreateConnectors_Should_Return_200OK()
        {
            // Arrange
            var (groupId, chargeStationId) = SeedData();
            var model = new Connectors() { List = new List<Connector> { new Connector(2, 15) } };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{_controllerBaseRoute}/{chargeStationId}/connectors", content);

            // Assert
            response.EnsureSuccessStatusCode();
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var chargeStation = dataContext.ChargeStations.Include(p=>p.Connectors).Single(p => p.Id == chargeStationId);
            chargeStation.Connectors.Should().ContainEquivalentOf(model.List.Single());

        }



        [Fact]
        public async Task Update_Should_Return_204NoContent_When_Exists()
        {
            // Arrange
            var (_, existingChargeStationId) = SeedData();
            var model = new UpdateChargeStation() { Name = "CS" };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"{_controllerBaseRoute}/{existingChargeStationId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        [Fact]
        public async Task UpdateConnectors_Should_Return_204NoContent_When_Exists()
        {
            // Arrange
            var (groupId, existingChargeStationId) = SeedData();
            var model = new Connectors() {  List = new List<Connector> { new Connector(1, 15) } };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"{_controllerBaseRoute}/{existingChargeStationId}/connectors", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var chargeStation = dataContext.ChargeStations.Include(p => p.Connectors).Single(p => p.Id == existingChargeStationId);
            chargeStation.Connectors.Should().ContainEquivalentOf(model.List.Single());
        }



        [Fact]
        public async Task Update_Should_Return_404NotFound_When_Does_Not_Exist()
        {
            // Arrange
            var nonExistentGroupId = Guid.NewGuid();
            var model = new CreateChargeStation() { Name = "CS", Connectors = new List<Connector> { new Connector(2, 15) } };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"{_controllerBaseRoute}/{nonExistentGroupId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_Should_Return_200OK_When_Exists()
        {
            // Arrange
            var (_, existingChargeStationId) = SeedData();

            // Act
            var response = await _client.GetAsync($"{_controllerBaseRoute}/{existingChargeStationId}");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_Should_Return_404NotFound_When_Does_Not_Exist()
        {
            // Arrange
            var nonExistentGroupId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"{_controllerBaseRoute}/{nonExistentGroupId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_Should_Return_200OK_When_Exists()
        {
            // Arrange
            var (_, existingChargeStationId) = SeedData();

            // Act
            var response = await _client.DeleteAsync($"{_controllerBaseRoute}/{existingChargeStationId}");

            // Assert
            response.EnsureSuccessStatusCode();
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dataContext.Groups.Where(c => c.Id == existingChargeStationId).ToList().Should().BeEmpty();
            //Check if Cascade delete is working
            dataContext.ChargeStations.Where(c => c.Group.Id == existingChargeStationId).ToList().Should().BeEmpty();
        }

        [Fact]
        public async Task Delete_Should_Return_404NotFound_When_Does_Not_Exist()
        {
            // Arrange
            var nonExistentGroupId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"{_controllerBaseRoute}/{nonExistentGroupId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task DeleteConnectors_Should_Return_200OK_When_Exists()
        {
            // Arrange
            var (_, existingChargeStationId) = SeedData();

            // Act
            var response = await _client.DeleteAsync($"{_controllerBaseRoute}/{existingChargeStationId}");

            // Assert
            response.EnsureSuccessStatusCode();
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dataContext.Groups.Where(c => c.Id == existingChargeStationId).ToList().Should().BeEmpty();
            //Check if Cascade delete is working
            dataContext.ChargeStations.Where(c => c.Group.Id == existingChargeStationId).ToList().Should().BeEmpty();
        }


        [Fact]
        public async Task DeleteConnectors_Should_Return_404NotFound_When_ChargeStation_Does_Not_Exist()
        {
            // Arrange
            var nonExistentChargeStationId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"{_controllerBaseRoute}/{nonExistentChargeStationId}/connectors");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task DeleteConnectors_Should_Return_404NotFound_When_Connectors_Does_Not_Exist()
        {
            // Arrange
            var (_, existingChargeStationId) = SeedData();
            var nonExistingConnectorId = 2;

            // Act
            var response = await _client.DeleteAsync($"{_controllerBaseRoute}/{existingChargeStationId}/connectors?connectorIds?{nonExistingConnectorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        
    }
}
