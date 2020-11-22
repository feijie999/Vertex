using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Transfer.Repository
{
    public class MigrationsDbContextFactory : IDesignTimeDbContextFactory<TransferDbContext>
    {
        public TransferDbContext CreateDbContext(string[] args)
        {
            var option = new DbContextOptionsBuilder<TransferDbContext>()
                .UseNpgsql(
                    "Server=localhost;Port=5432;Database=Vertex;User Id=postgres;Password=postgres;Pooling=true;MaxPoolSize=20;");
            return new TransferDbContext(option.Options);
        }
    }
}
