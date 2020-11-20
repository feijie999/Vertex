using System;
using System.Linq;
using IdGen;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using Vertex.Crud.Test.Biz.Repository;
using Vertex.Runtime;

namespace Vertex.Crud.Test.Core
{
    public class ClusterFixture : IDisposable
    {
        public ClusterFixture()
        {
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
            this.Cluster = builder.Build();
            this.Cluster.Deploy();
            var memorySQLiteConnection = new SqliteConnection("Data Source=EFCoreInMemory;Mode=Memory;Cache=Shared");
            memorySQLiteConnection.Open();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(memorySQLiteConnection);
            serviceCollection.AddVertex();
            serviceCollection.AddLogging();
            serviceCollection.AddEntityFrameworkSqlite()
                .AddDbContext<TestDbContext>(
                    options => { options.UseSqlite(memorySQLiteConnection); },
                    ServiceLifetime.Transient);
            this.Provider = serviceCollection.BuildServiceProvider();
            using (var db = this.Provider.GetService<TestDbContext>())
            {
                db.Database.EnsureCreated();
                db.Database.Migrate();
            }
        }

        public TestCluster Cluster { get; private set; }

        public ServiceProvider Provider { get; private set; }

        public IdGenerator ActorIdGen { get; } = new IdGenerator(0,
            new IdGeneratorOptions(sequenceOverflowStrategy: SequenceOverflowStrategy.SpinWait));

        public void Dispose()
        {
            this.Cluster.StopAllSilos();
            using (var db = this.Provider.GetService<TestDbContext>())
            {
                db.Database.EnsureDeleted();
            }
        }
    }
}