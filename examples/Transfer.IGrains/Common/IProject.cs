using Orleans;
using Transfer.IGrains.Dto;
using Vertex.Grain.EntityFramework.Abstractions;

namespace Transfer.IGrains.Common
{
    public interface IProject : ICrudGrain<ProjectDto>, IGrainWithGuidKey
    {
    }
}