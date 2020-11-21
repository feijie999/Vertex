using System;
using Vertex.Abstractions.Event;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework.Abstractions.Events
{
    [Serializable]
    public class DeletingSnapshotEvent<TSnapshot> : IEvent
        where TSnapshot : class, new()
    {
        public TSnapshot Snapshot { get; set; }

        public DeletingSnapshotEvent()
        {
        }

        public DeletingSnapshotEvent(TSnapshot snapshot)
            : this()
        {
            Snapshot = snapshot;
        }
    }
}
