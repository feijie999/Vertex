using System;
using System.Threading.Tasks;

namespace Vertex.Grain.EntityFramework.Abstractions
{
    public interface ICrudGrain<TSnapshotDto>
        where TSnapshotDto : class, new()
    {
        Task Create(TSnapshotDto snapshot, string flowId = "");

        Task<TSnapshotDto> Get();

        Task Update(TSnapshotDto snapshot, string flowId = "");

        Task Delete(string flowId = "");
    }
}