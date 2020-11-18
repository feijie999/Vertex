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
            Mapper = mapper;
        }

        //public override void Apply(SnapshotUnit<TPrimaryKey, TSnapshot> snapshotBox, EventUnit<TPrimaryKey> eventBox)
        //{

        //}

        //public override void CustomApply(Snapshot<TPrimaryKey, TSnapshot> snapshot, FullyEvent<TPrimaryKey> fullyEvent)
        //{
        //    Apply(snapshot.State, fullyEvent.Event);
        //}

        public void CreatingSnapshotHandle(TSnapshot snapshotState, CreatingSnapshotEvent<TSnapshot> evt)
        {
            Mapper.Map(evt.Snapshot, snapshotState);
        }

        public void UpdatingSnapshotHandle(TSnapshot snapshotState, UpdatingSnapshotEvent<TSnapshot> evt)
        {
            Mapper.Map(evt.Snapshot, snapshotState);
        }

        public void DeletingSnapshotHandle(TSnapshot snapshotState, DeletingSnapshotEvent<TPrimaryKey> evt)
        {
            var defaultSnapshot = new TSnapshot();
            Mapper.Map(defaultSnapshot, snapshotState);
        }

        #region Implementation of ICrudHandle<in TSnapshot>

        public virtual void Apply(TSnapshot snapshot, IEvent @event)
        {
            switch (@event)
            {
                case CreatingSnapshotEvent<TSnapshot> evt:
                    CreatingSnapshotHandle(snapshot, evt);
                    break;
                case UpdatingSnapshotEvent<TSnapshot> evt:
                    UpdatingSnapshotHandle(snapshot, evt);
                    break;
                case DeletingSnapshotEvent<TPrimaryKey> evt:
                    DeletingSnapshotHandle(snapshot, evt);
                    break;
            }
        }

        #endregion
    }
}
