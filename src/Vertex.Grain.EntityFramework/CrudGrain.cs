﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertex.Runtime.Actor;

namespace Vertex.Grain.EntityFramework
{
    public abstract class CrudGrain<TPrimaryKey, TSnapshotType, TEntityType, TSnapshotDto> :
            VertexActor<TPrimaryKey, TSnapshotType>,
            ICrudGrain<TSnapshotDto>
            where TSnapshotType : class, ISnapshot, TEntityType, new()
            where TEntityType : class, new()
            where TSnapshotDto : class, new()
            where TPrimaryKey : new()
    {
        protected IMapper Mapper { get; private set; }

        protected override async ValueTask CreateSnapshot()
        {
            //using (var repository = ServiceProvider.GetService<IGrainRepository<TEntityType, TPrimaryKey>>())
            //{
            //    var entity = await repository.FirstOrDefaultAsync(GrainId);
            //    if (entity != null)
            //    {
            //        Snapshot = new Snapshot<TPrimaryKey, TSnapshotType>(GrainId)
            //        {
            //            State = Mapper.Map<TSnapshotType>(entity)
            //        };
            //    }
            //    else
            //    {
            //        await base.CreateSnapshot();
            //    }
            //}
        }

        protected override ValueTask DependencyInjection()
        {
            this.Mapper = this.ServiceProvider.GetService<IMapper>();
            return base.DependencyInjection();
        }

        #region Implementation of ICrudGrain<TSnapshotDto>

        public virtual Task Create(TSnapshotDto snapshot)
        {
            var snapshotState = this.Mapper.Map<TSnapshotType>(snapshot);
            var evt = new CreatingSnapshotEvent<TSnapshotType>(snapshotState);
            return this.RaiseEvent(evt);
        }

        public virtual Task<TSnapshotDto> Get()
        {
            return Task.FromResult(this.Mapper.Map<TSnapshotDto>(this.Snapshot.Data));
        }

        public virtual Task Update(TSnapshotDto snapshot)
        {
            var snapshotState = this.Mapper.Map<TSnapshotType>(snapshot);
            var evt = new UpdatingSnapshotEvent<TSnapshotType>(snapshotState);
            return this.RaiseEvent(evt);
        }

        public virtual async Task Delete()
        {
            var evt = new DeletingSnapshotEvent<TPrimaryKey>(this.ActorId);
            await this.OnDeactivateAsync();
            await this.RaiseEvent(evt);
        }

        #endregion
    }
}
