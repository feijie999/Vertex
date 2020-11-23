using System;
using Vertext.Abstractions.Event;

namespace Vertex
{
    [Serializable]
    public class UpdatingEvent<TSnapshot> : IEvent
        where TSnapshot : class, new()
    {
        public TSnapshot Snapshot { get; set; }

        public UpdatingEvent()
        {
        }

        public UpdatingEvent(TSnapshot snapshot)
        {
            Snapshot = snapshot;
        }
    }
}