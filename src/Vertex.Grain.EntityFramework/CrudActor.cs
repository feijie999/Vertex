using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Vertex.Abstractions.Serialization;
using Vertex.Grain.EntityFramework.Abstractions;

namespace Vertex.Grain.EntityFramework
{
    public abstract class CrudActor<TPrimaryKey, TSnapshotType, TEntityType, TSnapshotDto, TDbContext> : Orleans.Grain,
        ICrudActor<TSnapshotDto>
        where TSnapshotType : class, new()
        where TEntityType : class, new()
        where TSnapshotDto : class, new()
        where TDbContext : DbContext
    {
        protected CrudActor()
        {
            ActorType = GetType();
        }

        protected TPrimaryKey ActorId { get; private set; }
        
        protected ISerializer Serializer { get; private set; }
        protected ILogger Logger { get; private set; }
        
        protected IMapper Mapper { get; private set; }
        
        protected Type ActorType { get; }
        
        protected TSnapshotType Snapshot { get; set; }
        
        public override async Task OnActivateAsync()
        {
            var type = typeof(TPrimaryKey);
            if (type == typeof(long) && this.GetPrimaryKeyLong() is TPrimaryKey longKey)
            {
                ActorId = longKey;
            }
            else if (type == typeof(string) && this.GetPrimaryKeyString() is TPrimaryKey stringKey)
            {
                ActorId = stringKey;
            }
            else if (type == typeof(Guid) && this.GetPrimaryKey() is TPrimaryKey guidKey)
            {
                ActorId = guidKey;
            }
            else
            {
                throw new ArgumentOutOfRangeException(typeof(TPrimaryKey).FullName);
            }
            
            DependencyInjection();
            await base.OnActivateAsync();
            await CreateSnapshot();
        }
        
        protected virtual void DependencyInjection()
        {
            Logger = (ILogger)ServiceProvider.GetService(typeof(ILogger<>).MakeGenericType(this.ActorType));
            Serializer = ServiceProvider.GetService<ISerializer>();
            Mapper = ServiceProvider.GetService<IMapper>();
        }
        
        protected virtual async ValueTask CreateSnapshot()
        {
            await ReloadFromDb();
        }

        protected abstract Expression<Func<TEntityType, bool>> FindEntityExpression { get; }
        
        protected virtual TDbContext GetDbContext()
        {
            return ServiceProvider.GetService<TDbContext>();
        }
        
        public virtual async Task Create(TSnapshotDto dto)
        {
            var entity = ConvertToEntity(dto);
            await using var db = GetDbContext();
            db.Set<TEntityType>().Add(entity);
            await db.SaveChangesAsync();
            Snapshot = ConvertToSnapshot(dto);
        }

        public virtual Task<TSnapshotDto> Get()
        {
            return Task.FromResult(ConvertToDto(Snapshot));
        }

        public async Task Update(TSnapshotDto dto)
        {
            var entity = ConvertToEntity(dto);
            await using var db = GetDbContext();
            db.Set<TEntityType>().Attach(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
            Snapshot = ConvertToSnapshot(dto);
        }

        public virtual async Task Delete()
        {
            await using var db = GetDbContext();
            var entity = await db.Set<TEntityType>().Where(FindEntityExpression).FirstOrDefaultAsync();
            if (entity !=null)
            {
                db.Set<TEntityType>().Remove(entity);
                await db.SaveChangesAsync();
                Snapshot = null;
            }
        }

        public virtual async Task ReloadFromDb()
        {
            await using var dbContext = this.GetDbContext();
            Snapshot = await dbContext.Set<TEntityType>().AsNoTracking().Where(FindEntityExpression)
                .ProjectTo<TSnapshotType>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        protected virtual TEntityType ConvertToEntity(TSnapshotDto dto)
        {
            return Mapper.Map<TEntityType>(dto);
        }
        
        protected virtual TSnapshotType ConvertToSnapshot(TSnapshotDto dto)
        {
            return Mapper.Map<TSnapshotType>(dto);
        }
        
        protected virtual TSnapshotDto ConvertToDto(TSnapshotType snapshot)
        {
            return Mapper.Map<TSnapshotDto>(snapshot);
        }
    }
}