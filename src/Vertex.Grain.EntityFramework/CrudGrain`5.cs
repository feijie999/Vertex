using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Abstractions.Snapshot;

namespace Vertex.Grain.EntityFramework
{
    public abstract class CrudGrain<TPrimaryKey, TSnapshotType, TEntityType, TSnapshotDto, TDbContext> :
        CrudGrain<TPrimaryKey, TSnapshotType, TSnapshotDto>
        where TSnapshotType : class, ISnapshot, TEntityType, new()
        where TEntityType : class, new()
        where TSnapshotDto : class, new()
        where TDbContext : DbContext
    {
        protected IMapper Mapper { get; private set; }

        protected virtual TDbContext GetDbContext()
        {
            return this.ServiceProvider.GetService<TDbContext>();
        }

        public abstract Expression<Func<TEntityType, bool>> FindEntityExpression { get; }

        protected override async ValueTask CreateSnapshot()
        {
            await base.CreateSnapshot();
            using (var dbContext = this.GetDbContext())
            {
                var snapshot = await dbContext.Set<TEntityType>().AsNoTracking().Where(FindEntityExpression)
                    .ProjectTo<TSnapshotType>(Mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
                if (snapshot != null)
                {
                    this.Snapshot.Data = snapshot;
                }
            }
        }

        protected override ValueTask DependencyInjection()
        {
            this.Mapper = this.ServiceProvider.GetService<IMapper>();
            return base.DependencyInjection();
        }
    }
}