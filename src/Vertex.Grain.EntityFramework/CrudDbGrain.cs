using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertex.Runtime.Actor;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework
{
    public abstract class CrudDbGrain<TPrimaryKey, TSnapshot, TEntityType, TDbContext> : FlowActor<TPrimaryKey>
        where TSnapshot : class, ISnapshot, new()
        where TEntityType : class, new()
        where TDbContext : DbContext
    {
        protected ICrudHandler<TPrimaryKey, TSnapshot> crudHandle;
        protected IMapper mapper;

        protected abstract TDbContext GetDbContext();

        protected override ValueTask DependencyInjection()
        {
            this.crudHandle = this.ServiceProvider.GetService<ICrudHandler<TPrimaryKey, TSnapshot>>();
            this.mapper = this.ServiceProvider.GetService<IMapper>();
            return base.DependencyInjection();
        }

        protected override async ValueTask OnEventDelivered(EventUnit<TPrimaryKey> eventUnit)
        {
            switch (eventUnit.Event)
            {
                case CreatingSnapshotEvent<TSnapshot> evt:
                    await this.CreatingSnapshotHandle(evt);
                    break;
                case UpdatingSnapshotEvent<TSnapshot> evt:
                    await this.UpdatingSnapshotHandle(evt);
                    break;
                case DeletingSnapshotEvent<TPrimaryKey> evt:
                    await this.DeletingSnapshotHandle(evt);
                    break;
            }
            await base.OnEventDelivered(eventUnit);
        }

        protected virtual async Task CreatingSnapshotHandle(CreatingSnapshotEvent<TSnapshot> evt)
        {
            await using var dbContext = this.GetDbContext();
            var entity = this.mapper.Map<TEntityType>(evt.Snapshot);
            await dbContext.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        protected virtual async Task UpdatingSnapshotHandle(UpdatingSnapshotEvent<TSnapshot> evt)
        {
            await using var dbContext = this.GetDbContext();
            var entity = this.mapper.Map<TEntityType>(evt.Snapshot);
            dbContext.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        protected virtual async Task DeletingSnapshotHandle(DeletingSnapshotEvent<TPrimaryKey> evt)
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