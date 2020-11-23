using AutoMapper;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
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
                case CreatingEvent<TSnapshot>:
                    CreatingSnapshotHandle(snapshotBox.Data,(CreatingEvent<TSnapshot>)eventBox.Event);
                    return;
                case UpdatingEvent<TSnapshot>:
                    UpdatingSnapshotHandle(snapshotBox.Data, (UpdatingEvent<TSnapshot>) eventBox.Event);
                    return;
                case DeletingEvent<TSnapshot>:
                    DeletingSnapshotHandle(snapshotBox.Data, (DeletingEvent<TSnapshot>)eventBox.Event);
                    return;
                default:
                    base.Apply(snapshotBox, eventBox);
                    return;
            }
        }

        public void CreatingSnapshotHandle(TSnapshot snapshotState, CreatingEvent<TSnapshot> evt)
        {
            this.Mapper.Map(evt.Snapshot, snapshotState);
        }

        public void UpdatingSnapshotHandle(TSnapshot snapshotState, UpdatingEvent<TSnapshot> evt)
        {
            this.Mapper.Map(evt.Snapshot, snapshotState);
        }

        public void DeletingSnapshotHandle(TSnapshot snapshotState, DeletingEvent<TSnapshot> evt)
        {
            this.Mapper.Map(evt.Snapshot, snapshotState);
        }
    }
}
