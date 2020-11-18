using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Grain.EntityFramework.Abstractions.Events;
using Vertex.Runtime.Actor;

namespace Vertex.Grain.EntityFramework
{

    public abstract class
        CrudGrain<TPrimaryKey, TSnapshotType, TSnapshotDto> : CrudGrain<TPrimaryKey, TSnapshotType, TSnapshotType
            , TSnapshotDto>
        where TSnapshotType : class, ISnapshot, new()
        where TSnapshotDto : class, new()
        where TPrimaryKey : new()
    {
    }
}
