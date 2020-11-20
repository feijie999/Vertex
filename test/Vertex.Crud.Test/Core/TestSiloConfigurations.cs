using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.TestingHost;
using Vertex.Crud.Test.Biz.Repository;
using Vertex.Grain.EntityFramework;
using Vertex.Runtime;
using Vertex.Storage.Linq2db;
using Vertex.Storage.Linq2db.Core;
using Vertex.Stream.InMemory;

namespace Vertex.Crud.Test.Core
{
    public class TestSiloConfigurations : ISiloConfigurator
    {
        public const string TestConnectionName = "vertex";
        public const string DefaultSqliteConnectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";

        public void Configure(ISiloBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
                {
                    services.AddVertex();
                    services.AddInMemoryStream();
                    services.AddCrudGrain(new[]
                    {
                        this.GetType().Assembly
                    });
                    var memorySQLiteConnection = new SqliteConnection("Data Source=EFCoreInMemory;Mode=Memory;Cache=Shared");
                    memorySQLiteConnection.Open();
                    services.AddSingleton(memorySQLiteConnection);
                    services.AddEntityFrameworkSqlite()
                        .AddDbContext<TestDbContext>(builder => { builder.UseSqlite(memorySQLiteConnection); },
                            ServiceLifetime.Transient);
                    services.AddLinq2DbStorage(config =>
                        {
                            var memorySQLiteConnection = new SqliteConnection(DefaultSqliteConnectionString);
                            memorySQLiteConnection.Open();
                            services.AddSingleton(memorySQLiteConnection);
                            config.Connections = new Storage.Linq2db.Options.ConnectionOptions[]
                            {
                                new Storage.Linq2db.Options.ConnectionOptions
                                {
                                    Name = TestConnectionName,
                                    ProviderName = "SQLite.MS",
                                    ConnectionString = DefaultSqliteConnectionString
                                }
                            };
                        },
                        new EventArchivePolicy("month",
                            (name, time) =>
                                $"Vertex_Archive_{name}_{DateTimeOffset.FromUnixTimeSeconds(time):yyyyMM}".ToLower(),
                            table => table.StartsWith("Vertex_Archive".ToLower())));
                }).AddSimpleMessageStreamProvider("SMSProvider", options => options.FireAndForgetDelivery = true)
                .AddMemoryGrainStorage("PubSubStore");
        }
    }
}