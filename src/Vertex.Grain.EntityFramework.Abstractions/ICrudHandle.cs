using System;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework.Abstractions
{
    public interface ICrudHandle<TPrimaryKey, TSnapshot>
        where TSnapshot : class, new()
    {
        void Apply(TSnapshot snapshot, IEvent evt);

        void CreatingSnapshotHandle(TSnapshot snapshotState, CreatingSnapshotEvent<TSnapshot> evt);
    }
}
