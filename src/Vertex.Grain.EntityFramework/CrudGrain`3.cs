using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertex.Runtime.Actor;

namespace Vertex.Grain.EntityFramework
{
    public abstract class CrudGrain<TPrimaryKey, TSnapshotType, TSnapshotDto> :
        VertexActor<TPrimaryKey, TSnapshotType>,
        ICrudGrain<TSnapshotDto>
        where TSnapshotType : class, ISnapshot, new()
        where TSnapshotDto : class, new()
    {
        protected IMapper Mapper { get; private set; }

        protected override ValueTask DependencyInjection()
        {
            this.Mapper = this.ServiceProvider.GetService<IMapper>();
            return base.DependencyInjection();
        }

        #region Implementation of ICrudGrain<TSnapshotDto>

        public virtual Task Create(TSnapshotDto snapshot, string flowId = "")
        {
            var snapshotState = this.Mapper.Map<TSnapshotType>(snapshot);
            var evt = new CreatingSnapshotEvent<TSnapshotType>(snapshotState);
            return this.RaiseEvent(evt, flowId);
        }

        public virtual Task<TSnapshotDto> Get()
        {
            return Task.FromResult(this.Mapper.Map<TSnapshotDto>(this.Snapshot.Data));
        }

        public virtual Task Update(TSnapshotDto snapshot, string flowId = "")
        {
            var snapshotState = this.Mapper.Map<TSnapshotType>(snapshot);
            var evt = new UpdatingSnapshotEvent<TSnapshotType>(snapshotState);
            return this.RaiseEvent(evt, flowId);
        }

        public virtual async Task Delete(string flowId = "")
        {
            var evt = new DeletingSnapshotEvent<TSnapshotType>(Snapshot.Data);
            await this.OnDeactivateAsync();
            await this.RaiseEvent(evt, flowId);
        }

        #endregion
    }
}