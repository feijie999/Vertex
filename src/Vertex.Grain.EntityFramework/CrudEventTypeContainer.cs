using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Vertex.Abstractions.Event;
using Vertex.Runtime.Serialization;
using Vertex.Utils;
using Vertext.Abstractions.Event;

namespace Vertex.Grain.EntityFramework
{
    public class CrudEventTypeContainer : IEventTypeContainer
    {
        private readonly ConcurrentDictionary<string, Type> nameDict = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentDictionary<Type, string> typeDict = new ConcurrentDictionary<Type, string>();
        private readonly ILogger<EventTypeContainer> logger;

        public CrudEventTypeContainer(ILogger<EventTypeContainer> logger)
        {
            this.logger = logger;
            var baseEventType = typeof(IEvent);
            var attributeType = typeof(EventNameAttribute);
            foreach (var assembly in AssemblyHelper.GetAssemblies(this.logger))
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (baseEventType.IsAssignableFrom(type))
                    {
                        string eventName;
                        var attribute = type.GetCustomAttributes(attributeType, false).FirstOrDefault();
                        if (attribute != null && attribute is EventNameAttribute nameAttribute
                                              && nameAttribute.Name != default)
                        {
                            eventName = nameAttribute.Name;
                            this.nameDict.TryAdd(eventName, type);
                            this.typeDict.TryAdd(type, eventName);
                        }
                    }
                }
            }
        }

        public bool TryGet(string name, out Type type)
        {
            var value = this.nameDict.GetOrAdd(name, key =>
            {
                foreach (var assembly in AssemblyHelper.GetAssemblies(this.logger))
                {
                    var type = assembly.GetType(name, false);
                    if (type != default)
                    {
                        return type;
                    }
                }

                return Type.GetType(name, false);
            });
            type = value;

            return value != default;
        }

        public bool TryGet(Type type, out string name)
        {
            var value = this.typeDict.GetOrAdd(type, key =>
            {
                return type.FullName;
            });

            name = value;
            return true;
        }
    }
}