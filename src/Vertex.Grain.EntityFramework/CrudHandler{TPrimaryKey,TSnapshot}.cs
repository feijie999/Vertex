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
                    CreatingSnapshotHandle(snapshotBox.Data,(CreatingSnapshotEvent<TSnapshot>)eventBox.Event);
                    return;
                case UpdatingSnapshotEvent<TSnapshot>:
                    UpdatingSnapshotHandle(snapshotBox.Data, (UpdatingSnapshotEvent<TSnapshot>) eventBox.Event);
                    return;
                case DeletingSnapshotEvent<TSnapshot>:
                    DeletingSnapshotHandle(snapshotBox.Data, (DeletingSnapshotEvent<TSnapshot>)eventBox.Event);
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

        public void DeletingSnapshotHandle(TSnapshot snapshotState, DeletingSnapshotEvent<TSnapshot> evt)
        {
            this.Mapper.Map(evt.Snapshot, snapshotState);
        }
    }
}
