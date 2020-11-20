using System.Threading.Tasks;
using Orleans;
using Vertex.Abstractions.Actor;
using Vertex.Runtime.Actor;
using Vertex.Runtime.Test.Events;
using Vertex.Runtime.Test.IActors;
using Vertex.Storage.Linq2db.Core;
using Vertex.Stream.Common;

namespace Vertex.Crud.Test.Biz.Actors
{
    [SnapshotStorage(Core.TestSiloConfigurations.TestConnectionName, nameof(AccountFlow), 3)]
    [StreamSub(nameof(Account), "flow", 3)]

    public sealed class AccountFlow : FlowActor<long>, IAccountFlow
    {
        private readonly IGrainFactory grainFactory;

        public AccountFlow(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        public override IVertexActor Vertex => this.grainFactory.GetGrain<IAccount>(this.ActorId);

        public Task EventHandle(TransferEvent evt)
        {
            var toActor = this.GrainFactory.GetGrain<IAccount>(evt.ToId);
            return toActor.TransferArrived(evt.Amount);
        }
    }
}
