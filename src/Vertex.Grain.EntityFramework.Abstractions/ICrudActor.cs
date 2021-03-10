using System.Threading.Tasks;
using Orleans.Concurrency;

namespace Vertex.Grain.EntityFramework.Abstractions
{
    public interface ICrudActor<TSnapshotDto>   
        where TSnapshotDto : class, new()
    {
        Task Create(TSnapshotDto dto);

        [AlwaysInterleave]
        Task<TSnapshotDto> Get();

        Task Update(TSnapshotDto dto);

        Task Delete();

        Task ReloadFromDb();
    }
}