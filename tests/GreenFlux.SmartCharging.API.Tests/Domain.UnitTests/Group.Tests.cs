using GreenFlux.SmartCharging.API.Domain;
using Dtos = GreenFlux.SmartCharging.API;
using Entities = GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GreenFlux.SmartCharging.API.Domain.Exceptions;
using GreenFlux.SmartCharging.API.Domain.Extensions;
using GreenFlux.SmartCharging.API.Domain.ValueObjects;
using AutoMapper.Internal;
using System.Xml.Linq;

namespace GreenFlux.SmartCharging.API.Tests.Domain.UnitTests
{
    public class GroupTests
    {
        private Group _sut;
        private string _groupName= "TestGroup";
        private int _groupCapacity= 100;

        public GroupTests() => _sut = new Group(_groupName, _groupCapacity);


        [Fact]
        public void Group_Initializes_WithNameAndCapacity()
        {
            //Assert
            _sut.Capacity.Value.Should().Be(_groupCapacity);
            _sut.Name.Should().Be(_groupName);
        }

        [Fact]
        public void Group_Initializes_WithIdNameAndCapacityAndChargeStations()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string name = "TestGroup";
            int capacity =  100;
            var chargeStations = new List<(Guid Id, string Name, IEnumerable<(int Id, int MaxCurrent)> Connectors)>  { (Guid.NewGuid(), "TestChargeStation", new List<(int Id, int MaxCurrent)> { (Id:1, MaxCurrent: 20), (Id:2, MaxCurrent:30) }) };

            //Act
            _sut = new Group(id, name, capacity, chargeStations);
            //Assert
            _sut.Id.Should().Be(id);
            _sut.Capacity.Value.Should().Be(capacity);
            _sut.Name.Should().Be(name);
            _sut.ChargeStations.Select(c=> new {c.Id,c.Name,Connectors = c.Connectors.Select(c => new { Id = c.Id.Value, MaxCurrent = c.MaxCurrent.Value }) }).Should().BeEquivalentTo(chargeStations.Select(p=> new { p.Id,p.Name,Connectors =p.Connectors.Select(c => new {c.Id,c.MaxCurrent})}));

        }

        [Fact]
        public void UpdateGroup_ValidNameAndCapacity_GroupUpdated()
        {
            // Arrange
            var name = "GroupUpdated";
            var capacity = 123;

            // Act
            _sut.Update(name, capacity);

            // Assert
            _sut.Name.Should().Be(name);
            _sut.Capacity.Value.Should().Be(capacity);
          
        }


        [Fact]
        public void UpdateGroup_CapacityIsAlreadyInUse_ThrowsCapacityInUseException()
        {
            _sut = new Group(Guid.NewGuid(), "Group", 100, new List<(Guid, string, IEnumerable<(int, int)>)> { ( Guid.NewGuid(),"TestChargeStation",new List<(int Id, int MaxCurrent)> { (1, 20), (2, 30) }) });

            // Act & Assert
            Assert.Throws<CapacityInUseException>(() => _sut.Update("Updated", 40));

        }



        [Fact]
        public void AddChargeStation_ValidChargeStation_ChargeStationAdded()
        {
            // Arrange
            var chargeStationName = "TestChargeStation";
            var connectors = new List<(int Id, int MaxCurrent)> { (1, 20), (2, 30) };

            // Act
            _sut.AddChargeStation(chargeStationName, connectors);

            // Assert
            _sut.ChargeStations.Should().HaveCount(1);
            var addedChargeStation = _sut.ChargeStations.Single();
            addedChargeStation.Name.Should().Be(chargeStationName);
            addedChargeStation.Connectors.Select(c => new { Id = c.Id.Value, MaxCurrent = c.MaxCurrent.Value }).Should().BeEquivalentTo(connectors.Select(c => new { c.Id, c.MaxCurrent }), options => options.WithStrictOrdering());
        }

        [Fact]
        public void AddChargeStation_InsufficientCapacity_ThrowsInsufficientGroupCapacityException()
        {
            // Arrange
            var connectors = new List<(int Id, int MaxCurrent)> { (1, 80), (2, 30) };
            _sut.AddChargeStation("ExistingChargeStation", new List<(int Id, int MaxCurrent)> { (1, 50) });

            // Act & Assert
            Assert.Throws<InsufficientGroupCapacityException>(() => _sut.AddChargeStation("TestChargeStation", connectors));
        }

        [Fact]
        public void UpdateChargeStationName_ValidChargeStation_ChargeStationNameUpdated()
        {
            // Arrange
            var chargeStation = (Name: "TestChargeStation",Connectors: new List<(int Id, int MaxCurrent)> { (1, 20), (2, 30) });
            _sut.AddChargeStation(chargeStation.Name, chargeStation.Connectors);
            var newName = "UpdatedChargeStationName";

            // Act
            _sut.UpdateChargeStation(_sut.ChargeStations.Last().Id, newName);

            // Assert
            var updatedChargeStation = _sut.ChargeStations.Last();
            updatedChargeStation.Name.Should().Be(newName);
        }

        [Fact]
        public void AddConnectors_ValidConnectors_ConnectorsAdded()
        {
            // Arrange
            var chargeStation = (Name: "TestChargeStation", Connectors: new List<(int Id, int MaxCurrent)> { (1, 20) });
            _sut.AddChargeStation(chargeStation.Name, chargeStation.Connectors);
            var chargeStationId = _sut.ChargeStations.Last().Id;
            var connectorsToAdd = new List<(int Id, int MaxCurrent)> { (2, 30), (3, 25) };
            var expectedConnectors = connectorsToAdd.Concat(chargeStation.Connectors).Select(c => new { c.Id, c.MaxCurrent });
            // Act
            _sut.AddConnectors(chargeStationId, connectorsToAdd);

            // Assert
            var updatedChargeStation = _sut.ChargeStations.Single(cs => cs.Id == _sut.ChargeStations.Last().Id);
            updatedChargeStation.Connectors.Select(c => new { Id = c.Id.Value, MaxCurrent = c.MaxCurrent.Value })
                .Should().BeEquivalentTo(expectedConnectors);
        }

        [Fact]
        public void AddConnectors_InsufficientCapacity_ThrowsInsufficientGroupCapacityException()
        {
            // Arrange

            var chargeStation = (Name: "ChargeStation", Connectors: new List<(int Id, int MaxCurrent)> { (1, 80) });
             _sut.AddChargeStation(chargeStation.Name, chargeStation.Connectors);
            var chargeStationId = _sut.ChargeStations.Last().Id;
            var connectorsToAdd = new List<(int Id, int MaxCurrent)> { (2, 30), (3, 25) };

            // Act & Assert
            Assert.Throws<InsufficientGroupCapacityException>(() => _sut.AddConnectors(chargeStationId, connectorsToAdd));
        }

        [Fact]
        public void RemoveConnectors_ValidConnectors_ConnectorsRemoved()
        {
            // Arrange
            var chargeStation = (Name: "TestChargeStation",Connectors: new List<(int Id, int MaxCurrent)> { (1, 20), (2, 30), (3, 25) });
            _sut.AddChargeStation(chargeStation.Name, chargeStation.Connectors);
            var chargeStationId = _sut.ChargeStations.Last().Id;
            var connectorsToRemove = new List<int> { 2, 3 };

            // Act
            _sut.RemoveConnectors(chargeStationId, connectorsToRemove);

            // Assert
            var updatedChargeStation = _sut.ChargeStations.Single(cs => cs.Id == chargeStationId);
            updatedChargeStation.Connectors.Select(c => c.Id.Value).Should().NotContain(connectorsToRemove);
        }

        [Fact]
        public void RemoveConnectors_NoConnectorLeft_ConnectorCountException()
        {
            // Arrange
            var chargeStation = (Name: "TestChargeStation", Connectors: new List<(int Id, int MaxCurrent)> { (1, 20) });
            _sut.AddChargeStation(chargeStation.Name, chargeStation.Connectors);
            var chargeStationId = _sut.ChargeStations.Last().Id;
            var connectorsToRemove = new List<int> { 1 };

            // Act & Assert
            Assert.Throws<ConnectorCountException>(() => _sut.RemoveConnectors(chargeStationId, connectorsToRemove));
        }

        [Fact]
        public void UpdateConnectors_ValidConnectors_ConnectorsUpdated()
        {
            // Arrange
            var chargeStation = (Name: "TestChargeStation",Connectors: new List<(int Id, int MaxCurrent)> { (1, 20), (2, 30) });
            _sut.AddChargeStation(chargeStation.Name, chargeStation.Connectors);
            var chargeStationId = _sut.ChargeStations.Last().Id;
            var updatedConnectors = new List<(int Id, int MaxCurrent)> { (1, 40), (2, 50) };

            // Act
            _sut.UpdateConnectors(chargeStationId, updatedConnectors);

            // Assert
            var updatedChargeStation = _sut.ChargeStations.Single(cs => cs.Id == chargeStationId);
            updatedChargeStation.Connectors.Single(c => c.Id.Value == 1).MaxCurrent.Value.Should().Be(40);
            updatedChargeStation.Connectors.Single(c => c.Id.Value == 2).MaxCurrent.Value.Should().Be(50);
        }

        [Fact]
        public void UpdateConnectors_InsufficientCapacity_ThrowsInsufficientGroupCapacityException()
        {
            // Arrange
            var chargeStation = (Name: "TestChargeStation", Connectors: new List<(int Id, int MaxCurrent)> { (1, 20), (2, 30) });
            _sut.AddChargeStation(chargeStation.Name, chargeStation.Connectors);
            var chargeStationId = _sut.ChargeStations.Last().Id;
            var updatedConnectors = new List<(int Id, int MaxCurrent)> { (1, 40), (2, 100) };

            // Act & Assert
            Assert.Throws<InsufficientGroupCapacityException>(() => _sut.UpdateConnectors(chargeStationId, updatedConnectors));
        }
    }



}
