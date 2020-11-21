using System;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework.Abstractions.Events
{
    [Serializable]
    public class UpdatingSnapshotEvent<TSnapshot> : IEvent
        where TSnapshot : class, new()
    {
        public TSnapshot Snapshot { get; set; }

        public UpdatingSnapshotEvent()
        {
        }

        public UpdatingSnapshotEvent(TSnapshot snapshot)
        {
            Snapshot = snapshot;
        }
    }
}