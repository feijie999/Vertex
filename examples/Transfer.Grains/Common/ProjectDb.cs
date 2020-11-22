using System;
using Transfer.Grains.Snapshot;
using Transfer.IGrains.Common;
using Transfer.Repository;
using Transfer.Repository.Entities;
using Vertex.Abstractions.Actor;
using Vertex.Grain.EntityFramework;
using Vertex.Storage.Linq2db.Core;
using Vertex.Stream.Common;

namespace Transfer.Grains.Common
{
    [SnapshotStorage(Consts.CoreDbName, nameof(ProjectDb), 3)]
    [StreamSub(nameof(Project), "db", 3)]
    public class ProjectDb : CrudDbGrain<string, ProjectSnapshot, ProjectEntity, TransferDbContext>, IProjectDb
    {
        public override IVertexActor Vertex => this.GrainFactory.GetGrain<IProject>(this.ActorId);
    }
}