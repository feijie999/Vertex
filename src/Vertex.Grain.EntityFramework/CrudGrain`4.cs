using Microsoft.EntityFrameworkCore;
using Vertex.Abstractions.Snapshot;

namespace Vertex.Grain.EntityFramework
{

    public abstract class
        CrudGrain<TPrimaryKey, TSnapshotType, TSnapshotDto, TDbContext> : CrudGrain<TPrimaryKey, TSnapshotType, TSnapshotType
            , TSnapshotDto, TDbContext>
        where TSnapshotType : class, ISnapshot, new()
        where TSnapshotDto : class, new()
        where TPrimaryKey : new()
        where TDbContext : DbContext
    {
    }
}
