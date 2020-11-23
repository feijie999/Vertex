using System;
using Vertex.Abstractions.Event;
using Vertext.Abstractions.Event;

namespace Vertex
{
    [Serializable]
    public class CreatingEvent<TSnapshot> : IEvent
        where TSnapshot : class, new()
    {
        public TSnapshot Snapshot { get; set; }

        public CreatingEvent()
        {
        }

        public CreatingEvent(TSnapshot snapshot) : this()
        {
            Snapshot = snapshot;
        }
    }
}
