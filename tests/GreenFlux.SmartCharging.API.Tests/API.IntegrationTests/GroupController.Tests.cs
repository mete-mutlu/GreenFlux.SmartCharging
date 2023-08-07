using FluentAssertions;
using System.Net;
using System.Text;
using Entities = GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using GreenFlux.SmartCharging.API.Persistence.EntityFramework;
using Request = GreenFlux.SmartCharging.API.Models.Request;
using Response = GreenFlux.SmartCharging.API.Models.Response;
using GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace GreenFlux.SmartCharging.API.Tests.API.IntegrationTests
{
    [Collection("IntegrationTestsCollection")]
    public class GroupControllerIntegrationTests 
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _controllerBaseRoute = "api/group";
        public GroupControllerIntegrationTests(TestHostFixture fixture)
        {
            _client = fixture.Client;
            _factory = fixture.Factory;
        }

        

        private Guid SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dataContext.Database.EnsureDeleted();
            dataContext.Database.EnsureCreated();
            var group = new Group { Name = "Existing Group", Capacity = 1000, ChargeStations = new List<ChargeStation> { new Entities.ChargeStation { Name = "Charge Station", Connectors = new List<Connector> { new Entities.Connector { Id = 1, MaxCurrent = 10 } } } } };
            dataContext.Add(group);
            dataContext.SaveChanges();
            return group.Id;
        }

  
        [Fact]
        public async Task Create_Should_Return_200OK()
        {
            // Arrange
            var model = new Request.Group
            {
                Name = "Group2",
                Capacity = 2000,
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(_controllerBaseRoute, content);

            // Assert
            response.EnsureSuccessStatusCode();

        }

        [Fact]
        public async Task Update_Should_Return_204NoContent_When_Exists()
        {
            // Arrange
            var existingGroupId = SeedData();
            var model = new Request.Group
            {
                Name = "Group1",
                Capacity = 10
            };

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"{_controllerBaseRoute}/{existingGroupId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var group = dataContext.Groups.Single(p => p.Id == existingGroupId);
            group.Capacity.Should().Be(model.Capacity);
            group.Name.Should().Be(model.Name);
        }

        [Fact]
        public async Task Update_Should_Return_404NotFound_When_Does_Not_Exist()
        {
            // Arrange
            var nonExistentGroupId = Guid.NewGuid();
            var model = new Request.Group
            {
                Name = "Updated",
                Capacity = 11
            };

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
            var existingGroupId = SeedData();

            // Act
            var response = await _client.GetAsync($"{_controllerBaseRoute}/{existingGroupId}");

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
            var existingGroupId = SeedData();

            // Act
            var response = await _client.DeleteAsync($"{_controllerBaseRoute}/{existingGroupId}");

            // Assert
            response.EnsureSuccessStatusCode();
            using var scope = _factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dataContext.Groups.Where(c => c.Id == existingGroupId).ToList().Should().BeEmpty();
            //Check if Cascade delete is working
            dataContext.ChargeStations.Where(c => c.Group.Id == existingGroupId).ToList().Should().BeEmpty();
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
        


    }
}
