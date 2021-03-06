﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdGen;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Transfer.IGrains.Common;
using Transfer.IGrains.Dto;
using Transfer.IGrains.DTx;

namespace Transfer.Client
{
    internal class Program
    {
        private static readonly IdGenerator IdGen = new IdGenerator(0);

        private static async Task Main(string[] args)
        {
            using var client = await StartClientWithRetries();

            while (true)
            {
                Console.WriteLine("Please select the type(1:normal,2:DTx,3:Crud)");
                var type = int.Parse(Console.ReadLine() ?? "1");
                if (type == 1)
                {
                    await Normal(client);
                }
                else if (type == 2)
                {
                    await DTx(client);
                }
                else if (type == 3)
                {
                    await Crud(client);
                }
            }
        }

        private static async Task Normal(IClusterClient client)
        {
            try
            {
                Console.WriteLine("Please enter the number of account");
                var accountCount = int.Parse(Console.ReadLine() ?? "10");
                Console.WriteLine("Please enter the number of executions");
                var times = int.Parse(Console.ReadLine() ?? "10");
                var topupTaskList = new List<Task>();
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    var actor1 = client.GetGrain<IAccount>(account);
                    await actor1.Create(0, IdGen.CreateId().ToString());
                    Console.WriteLine($"The balance of account {account} is{await actor1.GetBalance()}");
                    var actor2 = client.GetGrain<IAccount>(account + accountCount);
                    await actor2.Create(0, IdGen.CreateId().ToString());
                    Console.WriteLine($"The balance of account {account + accountCount} is{await actor2.GetBalance()}");
                }

                var topupWatch = new Stopwatch();
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    var actor1 = client.GetGrain<IAccount>(account);
                    topupTaskList.AddRange(Enumerable.Range(0, times).Select(async x =>
                    {
                        await actor1.TopUp(100, IdGen.CreateId().ToString());
                    }));
                }

                topupWatch.Start();
                await Task.WhenAll(topupTaskList);
                topupWatch.Stop();
                Console.WriteLine($"{times * accountCount} Recharge completed, taking: {topupWatch.ElapsedMilliseconds}ms");
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    Console.WriteLine($"The balance of account {account} is{await client.GetGrain<IAccount>(account).GetBalance()}");
                }

                var transferWatch = new Stopwatch();
                var transferTaskList = new List<Task>();
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    transferTaskList.AddRange(Enumerable.Range(0, times).Select(async x =>
                    {
                        var actor = client.GetGrain<IAccount>(account);
                        await actor.Transfer(account + accountCount, 50, IdGen.CreateId().ToString());
                    }));
                }

                transferWatch.Start();
                await Task.WhenAll(transferTaskList);
                transferWatch.Stop();
                Console.WriteLine(
                    $"{times * accountCount}The transfer is completed, taking: {transferWatch.ElapsedMilliseconds}ms");
                await Task.Delay(500);
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    Console.WriteLine($"The balance of account {account} is{await client.GetGrain<IAccount>(account).GetBalance()}");
                }

                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    Console.WriteLine($"The balance of account {account + accountCount} is{await client.GetGrain<IAccount>(account + accountCount).GetBalance()}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static async Task DTx(IClusterClient client)
        {
            try
            {
                Console.WriteLine("Please enter the number of account");
                var accountCount = int.Parse(Console.ReadLine() ?? "10");
                Console.WriteLine("Please enter the number of executions");
                var times = int.Parse(Console.ReadLine() ?? "10");
                var topupWatch = new Stopwatch();
                var topupTaskList = new List<Task>();
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    topupTaskList.AddRange(Enumerable.Range(0, times).Select(x => client.GetGrain<IDTxAccount>(account).TopUp(100, IdGen.CreateId().ToString())));
                }

                topupWatch.Start();
                await Task.WhenAll(topupTaskList);
                topupWatch.Stop();
                Console.WriteLine($"{times * accountCount} Recharge completed, taking: {topupWatch.ElapsedMilliseconds}ms");
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    Console.WriteLine($"The balance of account {account} is{await client.GetGrain<IDTxAccount>(account).GetBalance()}");
                }

                var transferWatch = new Stopwatch();
                var transferTaskList = new List<Task<bool>>();

                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    var txUnit = client.GetGrain<ITransferDtxUnit>(account / 50);
                    transferTaskList.AddRange(Enumerable.Range(0, times).Select(x => txUnit.Ask(new TransferRequest
                    {
                        FromId = account,
                        ToId = account + accountCount,
                        Amount = 50,
                        Id = IdGen.CreateId().ToString(),
                    })));
                }

                transferWatch.Start();
                await Task.WhenAll(transferTaskList);
                transferWatch.Stop();
                Console.WriteLine(
                    $"{times * accountCount}The transfer is completed, taking: {transferWatch.ElapsedMilliseconds}ms");
                await Task.Delay(500);
                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    Console.WriteLine($"The balance of account {account} is{await client.GetGrain<IDTxAccount>(account).GetBalance()}");
                }

                foreach (var account in Enumerable.Range(0, accountCount))
                {
                    Console.WriteLine($"The balance of account {account} is{await client.GetGrain<IDTxAccount>(account + accountCount).GetBalance()}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static async Task Crud(IClusterClient client)
        {
            try
            {
                Console.WriteLine("Please enter the number of executions");
                var times = int.Parse(Console.ReadLine() ?? "10");
                var idDic = new Dictionary<int, Guid>();
                foreach (var i in Enumerable.Range(0, times))
                {
                    idDic.Add(i, Guid.NewGuid());
                }
                var taskList = new List<Task>();
                taskList.AddRange(Enumerable.Range(0, times).Select(async x =>
                {
                    var actor = client.GetGrain<IProject>(idDic[x]);
                    await actor.Create(new ProjectDto()
                    {
                        CreateTime = DateTime.Now,
                        Id = idDic[x],
                        Name = "name" + x
                    }, Guid.NewGuid().ToString());
                }));
                await Task.WhenAll(taskList);
                taskList.Clear();
                taskList.AddRange(Enumerable.Range(0, times).Select(async x =>
                {
                    var actor = client.GetGrain<IProject>(idDic[x]);
                    var dto = await actor.Get();
                    Console.WriteLine("Projct is :" + JsonConvert.SerializeObject(dto));
                }));
                await Task.WhenAll(taskList);
                taskList.Clear();
                taskList.AddRange(Enumerable.Range(0, times).Select(async x =>
                {
                    var actor = client.GetGrain<IProject>(idDic[x]);
                    await actor.Update(new ProjectDto()
                    {
                        Id = idDic[x],
                        Name = "name" + x + "Updated"
                    }, Guid.NewGuid().ToString());
                }));
                await Task.WhenAll(taskList);
                taskList.Clear();

                taskList.AddRange(Enumerable.Range(0, times).Select(async x =>
                {
                    var actor = client.GetGrain<IProject>(idDic[x]);
                    var dto = await actor.Get();
                    Console.WriteLine("Projct updated is :" + JsonConvert.SerializeObject(dto));
                }));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static async Task<IClusterClient> StartClientWithRetries(int initializeAttemptsBeforeFailing = 5)
        {
            int attempt = 0;
            IClusterClient client;
            while (true)
            {
                try
                {
                    var builder = new ClientBuilder()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "Transfer";
                        })
                        //.UseLocalhostClustering()
                        .UseAdoNetClustering(delegate (AdoNetClusteringClientOptions options)
                        {
                            options.Invariant = "Npgsql";
                            options.ConnectionString =
                                "Server=localhost;Port=5432;Database=Vertex;User Id=postgres;Password=postgres;Pooling=true;MaxPoolSize=20;";
                        })
                        .ConfigureApplicationParts(parts =>
                            parts.AddApplicationPart(typeof(IAccount).Assembly).WithReferences())
                        .ConfigureLogging(logging => logging.AddConsole());
                    client = builder.Build();
                    await client.Connect();
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Console.WriteLine(
                        $"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            return client;
        }
    }
}
