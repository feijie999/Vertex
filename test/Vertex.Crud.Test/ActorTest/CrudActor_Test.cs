using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Orleans.TestingHost;
using Vertex.Crud.Test.Biz.Repository;
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
        private readonly Core.ClusterFixture fixture;

        public CrudActor_Test(Core.ClusterFixture fixture)
        {
            this.fixture = fixture;
            this.cluster = fixture.Cluster;
        }

        /// <summary>
        /// Create测试
        /// </summary>
        /// <param name="balance">金额</param>
        /// <param name="id">账户id</param>
        /// <returns></returns>
        [Theory]
        [InlineData(100, 1)]
        [InlineData(300, 2)]
        [InlineData(400, 3)]
        [InlineData(600, 4)]
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

        /// <summary>
        /// Update测试
        /// </summary>
        /// <param name="balance">金额</param>
        /// <param name="id">账户id</param>
        /// <returns></returns>
        [Theory]
        [InlineData(100, 1)]
        [InlineData(200, 2)]
        [InlineData(300, 3)]
        [InlineData(400, 4)]
        public async Task Update(decimal balance, int id)
        {
            var accountActor = this.cluster.GrainFactory.GetGrain<IAccount>(id);
            var dto = new AccountSnapshot()
            {
                Id = id
            };
            await accountActor.Create(dto, Guid.NewGuid().ToString());
            var snapshot = await accountActor.Get();
            Assert.Equal(snapshot.Balance, dto.Balance);
            dto.Balance = balance;
            await accountActor.Update(dto, Guid.NewGuid().ToString());
            snapshot = await accountActor.Get();
            Assert.Equal(snapshot.Balance, dto.Balance);
        }

        // [Theory]
        // [InlineData(1)]
        // [InlineData(2)]
        // public async Task Delete(int id)
        // {
        //     var accountActor = this.cluster.GrainFactory.GetGrain<IAccount>(id);
        //     var dto = new AccountSnapshot()
        //     {
        //         Id = id,
        //         Balance = 10
        //     };
        //     await accountActor.Create(dto, Guid.NewGuid().ToString());
        //     var snapshot = await accountActor.Get();
        //     Assert.Equal(snapshot.Balance, dto.Balance);
        //
        //     await accountActor.Delete(Guid.NewGuid().ToString());
        //     snapshot = await accountActor.Get();
        //     Assert.Equal(0, snapshot.Balance);
        // }
    }
}