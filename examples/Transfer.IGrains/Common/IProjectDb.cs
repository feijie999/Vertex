using Orleans;
using Vertex.Abstractions.Actor;

namespace Transfer.IGrains.Common
{
    public interface IProjectDb : IFlowActor, IGrainWithGuidKey
    {
    }
}