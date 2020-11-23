using System;
using Vertex.Abstractions.Snapshot;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework.Abstractions
{
    public interface ICrudHandler<TPrimaryKey, TSnapshot>
        where TSnapshot : class, ISnapshot, new()
    {
        void Apply(SnapshotUnit<TPrimaryKey, TSnapshot> snapshotBox, EventUnit<TPrimaryKey> eventBox);

        void CreatingSnapshotHandle(TSnapshot snapshotState, CreatingEvent<TSnapshot> evt);

        void UpdatingSnapshotHandle(TSnapshot snapshotState, UpdatingEvent<TSnapshot> evt);

        void DeletingSnapshotHandle(TSnapshot snapshotState, DeletingEvent<TSnapshot> evt);
    }
}
