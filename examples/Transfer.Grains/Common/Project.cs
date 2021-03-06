﻿using System;
using System.Linq.Expressions;
using Transfer.Grains.Snapshot;
using Transfer.IGrains.Common;
using Transfer.IGrains.Dto;
using Transfer.Repository;
using Transfer.Repository.Entities;
using Vertex.Grain.EntityFramework;
using Vertex.Storage.Linq2db.Core;
using Vertex.Stream.Common;

namespace Transfer.Grains.Common
{
    [EventStorage(Consts.CoreDbName, nameof(Project), 3)]
    [EventArchive(Consts.CoreDbName, nameof(Project), "month")]
    [SnapshotStorage(Consts.CoreDbName, nameof(Project), 3)]
    [Stream(nameof(Project), 3)]
    public class Project : CrudGrain<Guid, ProjectSnapshot, ProjectEntity, ProjectDto, TransferDbContext>, IProject
    {
        public override Expression<Func<ProjectEntity, bool>> FindEntityExpression => x => x.Id == ActorId;
    }
}