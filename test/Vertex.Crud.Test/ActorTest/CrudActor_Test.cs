using System;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Runtime;
using Orleans.TestingHost;
using Vertex.Crud.Test.Biz.Snapshot;
using Vertex.Runtime;
using Vertex.Runtime.Test.IActors;
using Xunit;

namespace Vertex.Crud.Test.ActorTest
{
    [Collection(Core.ClusterCollection.Name)]
    public class CrudActor_Test
    {
        private readonly TestCluster cluster;

        public CrudActor_Test(Core.ClusterFixture fixture)
        {
            this.cluster = fixture.Cluster;
        }

        /// <summary>
        /// Create测试
        /// </summary>
        /// <param name="count">测试次数</param>
        /// <param name="id">账户id</param>
        /// <returns></returns>
        [Theory]
        [InlineData(100, 1)]
        [InlineData(500, 2)]
        [InlineData(1000, 3)]
        [InlineData(3000, 4)]
        public async Task Create(decimal balance, int id)
        {
            var accountActor = this.cluster.GrainFactory.GetGrain<IAccount>(id);
            var dto = new AccountSnapshot()
            {
                Balance = balance,
                Id = id
            };
            await accountActor.Create(dto, Guid.NewGuid().ToString());
            var snapshot = await accountActor.Get();
            Assert.Equal(snapshot.Balance, dto.Balance);
        }
    }
}