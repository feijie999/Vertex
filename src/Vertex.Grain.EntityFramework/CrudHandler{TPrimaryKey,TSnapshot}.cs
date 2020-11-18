using System;
using AutoMapper;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertex.Runtime.Snapshot;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework
{
    public class CrudHandler<TPrimaryKey, TSnapshot> : SnapshotHandlerBase<TPrimaryKey, TSnapshot>,
               ICrudHandler<TPrimaryKey, TSnapshot>
               where TSnapshot : class, ISnapshot, new()
    {
        protected readonly IMapper Mapper;

        public CrudHandler(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        public override void Apply(SnapshotUnit<TPrimaryKey, TSnapshot> snapshotBox, EventUnit<TPrimaryKey> eventBox)
        {
            switch (eventBox.Event)
            {
                case CreatingSnapshotEvent<TSnapshot>:
                    return;
                case UpdatingSnapshotEvent<TSnapshot>:
                    return;
                case DeletingSnapshotEvent<TSnapshot>:
                    return;
                default:
                    base.Apply(snapshotBox, eventBox);
                    return;
            }
        }

        public void CreatingSnapshotHandle(TSnapshot snapshotState, CreatingSnapshotEvent<TSnapshot> evt)
        {
            this.Mapper.Map(evt.Snapshot, snapshotState);
        }

        public void UpdatingSnapshotHandle(TSnapshot snapshotState, UpdatingSnapshotEvent<TSnapshot> evt)
        {
            this.Mapper.Map(evt.Snapshot, snapshotState);
        }

        public void DeletingSnapshotHandle(TSnapshot snapshotState, DeletingSnapshotEvent<TPrimaryKey> evt)
        {
            var defaultSnapshot = new TSnapshot();
            this.Mapper.Map(defaultSnapshot, snapshotState);
        }
    }
}
