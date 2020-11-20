using Microsoft.EntityFrameworkCore;

namespace Vertex.Crud.Test.Biz.Repository
{
    public sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> contextOptions) : base(contextOptions)
        {
        }

        public static string ConnectionString { get; set; }

        public DbSet<AccountEntity> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>().Property(x => x.Id).ValueGeneratedNever();
            base.OnModelCreating(modelBuilder);
        }
    }
}