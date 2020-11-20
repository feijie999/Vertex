using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Vertex.Abstractions.Actor;
using Vertex.Crud.Test.Biz.Repository;
using Vertex.Crud.Test.Biz.Snapshot;
using Vertex.Grain.EntityFramework;
using Vertex.Runtime.Actor;
using Vertex.Runtime.Test.Events;
using Vertex.Runtime.Test.IActors;
using Vertex.Storage.Linq2db.Core;
using Vertex.Stream.Common;
using Vertext.Abstractions.Event;

namespace Vertex.Crud.Test.Biz.Actors
{
    [SnapshotStorage(Core.TestSiloConfigurations.TestConnectionName, nameof(AccountDb), 3)]
    [StreamSub(nameof(Account), "db", 3)]
    public sealed class AccountDb : CrudDbGrain<long, AccountSnapshot, AccountEntity, TestDbContext>, IAccountDb
    {
        private readonly IGrainFactory grainFactory;
        private int executedTimes;

        public AccountDb(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        public override IVertexActor Vertex => this.grainFactory.GetGrain<IAccount>(this.ActorId);

        public Task EventHandle(TransferEvent evt, EventMeta eventBase)
        {
            // Update database here
            return Task.CompletedTask;
        }

        public Task EventHandle(TopupEvent evt, EventMeta eventBase)
        {
            Interlocked.Increment(ref this.executedTimes);

            // Update database here
            return Task.CompletedTask;
        }

        public Task EventHandle(TransferArrivedEvent evt, EventMeta eventBase)
        {
            // Update database here
            return Task.CompletedTask;
        }

        public Task EventHandle(TransferRefundsEvent evt, EventMeta eventBase)
        {
            // Update database here
            return Task.CompletedTask;
        }
    }
}