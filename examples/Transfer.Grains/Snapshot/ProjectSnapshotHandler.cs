using System;
using AutoMapper;
using Vertex.Grain.EntityFramework;

namespace Transfer.Grains.Snapshot
{
    public class ProjectSnapshotHandler : CrudHandler<Guid, ProjectSnapshot>
    {
        public ProjectSnapshotHandler(IMapper mapper) : base(mapper)
        {
        }
    }
}