using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertex.Runtime.Actor;

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
