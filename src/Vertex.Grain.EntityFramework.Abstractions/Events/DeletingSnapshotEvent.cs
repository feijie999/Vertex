using System;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework.Abstractions.Events
{
    [Serializable]
    public class DeletingSnapshotEvent<TPrimaryKey> : IEvent
    {
        public TPrimaryKey PrimaryKey { get; set; }

        public DeletingSnapshotEvent()
        {
        }

        public DeletingSnapshotEvent(TPrimaryKey primaryKey) : this()
        {
            PrimaryKey = primaryKey;
        }
    }
}
