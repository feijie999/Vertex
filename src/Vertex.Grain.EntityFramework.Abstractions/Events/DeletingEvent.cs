using System;
using Vertex.Abstractions.Event;
using Vertext.Abstractions.Event;

namespace Vertex
{
    [Serializable]
    public class DeletingEvent<TSnapshot> : IEvent
        where TSnapshot : class, new()
    {
        public TSnapshot Snapshot { get; set; }

        public DeletingEvent()
        {
        }

        public DeletingEvent(TSnapshot snapshot)
            : this()
        {
            Snapshot = snapshot;
        }
    }
}
