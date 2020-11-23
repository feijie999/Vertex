using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Runtime.Actor;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework
{
    public abstract class CrudDbGrain<TPrimaryKey, TSnapshot, TEntityType, TDbContext> : FlowActor<TPrimaryKey>
        where TSnapshot : class, ISnapshot, new()
        where TEntityType : class, new()
        where TDbContext : DbContext
    {
        protected IGrainFactory GrainFactory;
        protected ICrudHandler<TPrimaryKey, TSnapshot> crudHandle;
        protected IMapper mapper;

        protected virtual TDbContext GetDbContext()
        {
            return this.ServiceProvider.GetService<TDbContext>();
        }

        protected override ValueTask DependencyInjection()
        {
            this.GrainFactory = this.ServiceProvider.GetService<IGrainFactory>();
            this.crudHandle = this.ServiceProvider.GetService<ICrudHandler<TPrimaryKey, TSnapshot>>();
            this.mapper = this.ServiceProvider.GetService<IMapper>();
            return base.DependencyInjection();
        }

        protected override async ValueTask OnEventDelivered(EventUnit<TPrimaryKey> eventUnit)
        {
            switch (eventUnit.Event)
            {
                case CreatingEvent<TSnapshot> evt:
                    await this.CreatingSnapshotHandle(evt);
                    break;
                case UpdatingEvent<TSnapshot> evt:
                    await this.UpdatingSnapshotHandle(evt);
                    break;
                case DeletingEvent<TSnapshot> evt:
                    await this.DeletingSnapshotHandle(evt);
                    break;
            }
            await base.OnEventDelivered(eventUnit);
        }

        protected virtual async Task CreatingSnapshotHandle(CreatingEvent<TSnapshot> evt)
        {
            await using var dbContext = this.GetDbContext();
            var entity = this.mapper.Map<TEntityType>(evt.Snapshot);
            await dbContext.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        protected virtual async Task UpdatingSnapshotHandle(UpdatingEvent<TSnapshot> evt)
        {
            await using var dbContext = this.GetDbContext();
            var entity = this.mapper.Map<TEntityType>(evt.Snapshot);
            dbContext.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        protected virtual async Task DeletingSnapshotHandle(DeletingEvent<TSnapshot> evt)
        {
            await using var dbContext = this.GetDbContext();
            var entity = dbContext.Find<TEntityType>(this.ActorId);
            if (entity != null)
            {
                dbContext.Remove(entity);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}