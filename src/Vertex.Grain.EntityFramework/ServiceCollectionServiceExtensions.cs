using System;
using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;

namespace Vertex.Grain.EntityFramework
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddCrudGrain(this IServiceCollection serviceCollection, Assembly[] assemblies, Action<IMapperConfigurationExpression> configAction = null)
        {
            serviceCollection.AddTransient(typeof(ICrudHandler<,>), typeof(CrudHandler<,>));
            serviceCollection.AddSingleton(typeof(ISnapshotHandler<,>), typeof(CrudHandler<,>));
            serviceCollection.AddAutoMapper(configAction, assemblies);
            return serviceCollection;
        }
    }
}