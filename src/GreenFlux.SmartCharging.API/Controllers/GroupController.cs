using AutoMapper;
using GreenFlux.SmartCharging.API.Attributes;
using GreenFlux.SmartCharging.API.Domain;
using GreenFlux.SmartCharging.API.Models.Response;
using GreenFlux.SmartCharging.API.Persistence.EntityFramework;
using GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Entities = GreenFlux.SmartCharging.API.Persistence.EntityFramework.Entities;
using Models = GreenFlux.SmartCharging.API.Models;
namespace GreenFlux.SmartCharging.API.Controllers
{

    /// <summary>
    /// Domain Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionFilter))]
    public class GroupController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public GroupController(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Models.Response.GroupResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Models.Response.GroupResponse>> Create(Models.Request.Group model)
        {
            var aggregateRoot = new Domain.Group(model.Name, model.Capacity);
            var group = _mapper.Map<Entities.Group>(aggregateRoot);
            _dataContext.Groups.Add(group);
            await _dataContext.SaveChangesAsync();
            return Ok(_mapper.Map<Models.Response.GroupResponse>(group));
        }



        [HttpPut("{id}")]
        
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, Models.Request.Group model)
        {
            var group = await GetGroup(id);

            if (group is null)
                return NotFound();
            var aggregateRoot = _mapper.Map<Domain.Group>(group);
            aggregateRoot.Update(model.Name, model.Capacity);
            _mapper.Map(aggregateRoot, group);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }



        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Models.Response.GroupResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupResponse>> Get(Guid id)
        {
            var group = await GetGroup(id);

            if (group is not null)
                return Ok(_mapper.Map<GroupResponse>(group));

            return NotFound();
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var group = await GetGroup(id);
            if (group is not null)
            {
                _dataContext.Remove(group);
                await _dataContext.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        private async Task<Entities.Group?> GetGroup(Guid id) => await _dataContext.Groups.Include(p => p.ChargeStations).ThenInclude(p => p.Connectors).FirstOrDefaultAsync(p => p.Id == id);
    }

}
