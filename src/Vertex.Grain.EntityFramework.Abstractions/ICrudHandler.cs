using System;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework.Abstractions
{
    public interface ICrudHandler<TPrimaryKey, TSnapshot>
        where TSnapshot : class, ISnapshot, new()
    {
        void Apply(SnapshotUnit<TPrimaryKey, TSnapshot> snapshotBox, EventUnit<TPrimaryKey> eventBox);

        void CreatingSnapshotHandle(TSnapshot snapshotState, CreatingSnapshotEvent<TSnapshot> evt);

        void UpdatingSnapshotHandle(TSnapshot snapshotState, UpdatingSnapshotEvent<TSnapshot> evt);

        void DeletingSnapshotHandle(TSnapshot snapshotState, DeletingSnapshotEvent<TSnapshot> evt);
    }
}
