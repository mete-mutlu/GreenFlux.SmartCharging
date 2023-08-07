using AutoMapper;
using GreenFlux.SmartCharging.API.Attributes;
using GreenFlux.SmartCharging.API.Domain;
using GreenFlux.SmartCharging.API.Persistence.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Request = GreenFlux.SmartCharging.API.Models.Request;
using Response = GreenFlux.SmartCharging.API.Models.Response;
using Entities = GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using GreenFlux.SmartCharging.API.Domain.ValueObjects;
using System.Linq;

namespace GreenFlux.SmartCharging.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    public class ChargeStationController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public ChargeStationController(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        [HttpPost($"{{{nameof(groupId)}}}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Response.ChargeStationResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response.ChargeStationResponse>> Create(Guid groupId, Request.CreateChargeStation model)
        {
            var group = await _dataContext.GetGroup(groupId);

            if (group is null)
                return NotFound("Group not found.");

            var aggregateRoot = _mapper.Map<Group>(group);
            aggregateRoot.AddChargeStation(model.Name, model.Connectors.Select(p => (p.Id, p.MaxCurrent)));
            var newChargeStation = _mapper.Map<Entities.ChargeStation>(aggregateRoot.ChargeStations.Last(), c => c.AfterMap((src, dest) => { dest.GroupId = groupId; }));

            _dataContext.ChargeStations.Add(newChargeStation);

            await _dataContext.SaveChangesAsync();

            return Ok(_mapper.Map<Response.ChargeStationResponse>(group.ChargeStations.Last()));
        }



        [HttpPost($"{{{nameof(id)}}}/connectors")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response.ChargeStationResponse>> Create(Guid id, Request.Connectors model)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();
            var group = await _dataContext.GetGroupByChargeStationId(id);
            if (group is null)
                return NotFound("Charge Station not found.");
            var aggregateRoot = _mapper.Map<Group>(group);
            aggregateRoot.AddConnectors(id, model.List.Select(p => new Tuple<int, int>(p.Id, p.MaxCurrent).ToValueTuple()));

            var connectorsToAdd = _mapper.Map<IEnumerable<Entities.Connector>>(aggregateRoot.ChargeStations.Single(p => p.Id == id).Connectors.Where(f => model.List.Select(k => k.Id).Contains(f.Id.Value)));
            foreach (var connector in connectorsToAdd)
                group.ChargeStations.Single(p => p.Id == id).Connectors.Add(connector);

            await _dataContext.SaveChangesAsync();
            transaction.Commit();
            return Ok(_mapper.Map<Response.ChargeStationResponse>(group.ChargeStations.Single(p => p.Id == id)));
        }

        [HttpPut($"{{{nameof(id)}}}")]
        
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, Request.UpdateChargeStation model)
        {

            var chargeStation = await _dataContext.GetChargeStation(id);

            if (chargeStation is null)
                return NotFound();

            chargeStation.Name = model.Name;
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut($"{{{nameof(id)}}}/connectors")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Response.ChargeStationResponse>> Update(Guid id, Request.Connectors model)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();
            var group = await _dataContext.GetGroupByChargeStationId(id);
            if (group is null)
                return NotFound("Group not found.");

            if (!group.ChargeStations.Single(p => p.Id == id).Connectors.All(p => model.List.Select(p=>p.Id).Contains(p.Id)))
                return NotFound("Connector(s) not found.");

            var aggregateRoot = _mapper.Map<Group>(group);
            aggregateRoot.UpdateConnectors(id, model.List.Select(p => new Tuple<int, int>(p.Id, p.MaxCurrent).ToValueTuple()));

            var connectorsToUpdate = aggregateRoot.ChargeStations.Single(p => p.Id == id).Connectors.Where(f => model.List.Select(k => k.Id).Contains(f.Id.Value));
            foreach (var connector in connectorsToUpdate)
                group.ChargeStations.Single(p => p.Id == id).Connectors.Single(p => p.Id == connector.Id.Value).MaxCurrent = connector.MaxCurrent.Value;
            
            await _dataContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return NoContent();
        }

        [HttpGet($"{{{nameof(id)}}}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Response.ChargeStationResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Response.ChargeStationResponse>> Get(Guid id)
        {
            var chargeStation = await _dataContext.GetChargeStation(id);

            if (chargeStation is not null)
                return Ok(_mapper.Map<Response.ChargeStationResponse>(chargeStation));

            return NotFound();
        }

        [HttpDelete($"{{{nameof(id)}}}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();
            var chargeStation = await _dataContext.GetChargeStation(id);
            if (chargeStation is not null)
            {
                _dataContext.Remove(chargeStation);
                await _dataContext.SaveChangesAsync();
                transaction.Commit();
                return Ok();
            }
            return NotFound();
        }


        [HttpDelete($"{{{nameof(chargeStationId)}}}/connectors")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteConnectors(Guid chargeStationId, [FromQuery] IEnumerable<int> connectorIds)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();
            var group = await _dataContext.GetGroupByChargeStationId(chargeStationId);
            if (group is not null)
            {
                if (!group.ChargeStations.Single(c => c.Id == chargeStationId).Connectors.Any(p => connectorIds.Contains(p.Id)))
                    return NotFound("Connector(s) not found");
                var aggregateRoot = _mapper.Map<Group>(group);
                aggregateRoot.RemoveConnectors(chargeStationId, connectorIds);
                _mapper.Map(aggregateRoot, group);
                await _dataContext.SaveChangesAsync();
                transaction.Commit();
                return Ok();
            }
            return NotFound("Group not found");
        }


    }
}
