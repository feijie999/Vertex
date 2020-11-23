using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Event;
using Vertex.Abstractions.Snapshot;
using Vertex.Grain.EntityFramework.Abstractions;
using Vertex.Utils;

namespace Vertex.Grain.EntityFramework
{
    public static class ServiceCollectionServiceExtensions
    {
        public static IServiceCollection AddCrudGrain(this IServiceCollection serviceCollection, Assembly[] assemblies, Action<IMapperConfigurationExpression> configAction = null)
        {
            serviceCollection.AddTransient(typeof(ICrudHandler<,>), typeof(CrudHandler<,>));
            serviceCollection.AddSingleton(typeof(ISnapshotHandler<,>), typeof(CrudHandler<,>));
            serviceCollection.AddSingleton<IEventTypeContainer, CrudEventTypeContainer>();
            serviceCollection.AddAutoMapper(expression =>
            {
                configAction?.Invoke(expression);
                var baseEventType = typeof(ISnapshot);
                foreach (var assembly in AssemblyHelper.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes().Where(x => baseEventType.IsAssignableFrom(x) && !x.IsAbstract))
                    {
                        expression.CreateMap(type, type);
                    }
                }
            }, assemblies);
            return serviceCollection;
        }
    }
}