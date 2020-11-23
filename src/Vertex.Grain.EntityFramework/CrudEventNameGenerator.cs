using System;
using System.Linq;
using Vertex.Abstractions.Event;

namespace Vertex.Grain.EntityFramework
{
    public class CrudEventNameGenerator : IEventNameGenerator
    {
        public const int MaxEventNameLength = 100;

        public string GetName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            if (type.FullName != null && type.FullName.Length > MaxEventNameLength)
            {
                var shortName = type.Name + string.Join('_', type.GetGenericArguments().Select(x => x.FullName));
                return shortName.Substring(shortName.Length - MaxEventNameLength < 0
                    ? 0
                    : shortName.Length - MaxEventNameLength);
            }
            else
            {
                return type.FullName;
            }
        }
    }
}