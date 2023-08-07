using AutoMapper;
using GreenFlux.SmartCharging.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GreenFlux.SmartCharging.API.Persistence.EntityFramework
{
    public class DataContext : DbContext
    {
        private readonly IMapper mapper;

        public DataContext(DbContextOptions<DataContext> options, IMapper mapper) : base(options)
        {
            this.mapper = mapper;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Connector>().HasKey(p => new { p.Id, p.ChargeStationId });
            
        }

        public DbSet<Entities.Group> Groups { get; set; }

        public DbSet<Entities.ChargeStation> ChargeStations { get; set; }

        public DbSet<Entities.Connector> Connectors { get; set; }

        public async Task<Entities.Group?> GetGroupByChargeStationId(Guid chargeStationId)
        {
            var groupId = (await ChargeStations.FirstOrDefaultAsync(p => p.Id == chargeStationId))?.GroupId;
            if (groupId is not null)
                return await GetGroup(groupId.Value);
            return null;
        }


        public async Task<Group?> GetAggregateRoot(Guid id)
        {
            var group = await Groups.Include(p => p.ChargeStations).ThenInclude(p => p.Connectors).FirstOrDefaultAsync(p => p.Id == id);

            if (group is null)
                return null;

            return new Group(group.Id, group.Name, group.Capacity, group.ChargeStations.Select(cs => (cs.Id, cs.Name, cs.Connectors.Select(c => (c.Id, c.MaxCurrent)))));
        }

        public async Task<Entities.Group?> GetGroup(Guid id) => await Groups.Include(p => p.ChargeStations).ThenInclude(p => p.Connectors).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Entities.ChargeStation?> GetChargeStation(Guid id) => await ChargeStations.Include(p => p.Connectors).FirstOrDefaultAsync(p => p.Id == id);
    }
}
