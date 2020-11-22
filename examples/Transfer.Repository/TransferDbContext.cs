using Microsoft.EntityFrameworkCore;
using Transfer.Repository.Entities;

namespace Transfer.Repository
{
    public sealed class TransferDbContext : DbContext
    {
        public TransferDbContext(DbContextOptions<TransferDbContext> contextOptions)
            : base(contextOptions)
        {
        }
        public static string ConnectionString { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<ProjectEntity> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(x => x.Id).ValueGeneratedNever();
            base.OnModelCreating(modelBuilder);
        }
    }
}