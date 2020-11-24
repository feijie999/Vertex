using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;

namespace Vertex.Grain.EntityFramework
{
    public abstract class CrudGrain<TPrimaryKey, TSnapshotType, TEntityType, TSnapshotDto, TDbContext> :
        CrudGrain<TPrimaryKey, TSnapshotType, TSnapshotDto>
        where TSnapshotType : class, ISnapshot, TEntityType, new()
        where TEntityType : class, new()
        where TSnapshotDto : class, new()
        where TDbContext : DbContext
    {
        protected IMapper Mapper { get; private set; }

        protected virtual TDbContext GetDbContext()
        {
            return this.ServiceProvider.GetService<TDbContext>();
        }

        protected override async ValueTask CreateSnapshot()
        {
            await base.CreateSnapshot();
            using (var dbContext = this.GetDbContext())
            {
                var entity = await dbContext.Set<TEntityType>().FindAsync(this.ActorId);
                if (entity != null)
                {
                    this.Snapshot.Data = this.ServiceProvider.GetService<IMapper>()
                        .Map<TEntityType, TSnapshotType>(entity);
                }
            }
        }

        protected override ValueTask DependencyInjection()
        {
            this.Mapper = this.ServiceProvider.GetService<IMapper>();
            return base.DependencyInjection();
        }
    }
}