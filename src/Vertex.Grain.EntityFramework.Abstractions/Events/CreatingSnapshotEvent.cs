using System;
using Vertex.Abstractions.Event;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework.Abstractions.Events
{
    [Serializable]
    public class CreatingSnapshotEvent<TSnapshot> : IEvent
        where TSnapshot : class, new()
    {
        public TSnapshot Snapshot { get; set; }

        public CreatingSnapshotEvent()
        {
        }

        public CreatingSnapshotEvent(TSnapshot snapshot) : this()
        {
            Snapshot = snapshot;
        }
    }
}
