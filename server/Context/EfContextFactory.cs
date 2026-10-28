using Context.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Context;

public class EfContextFactory : IDesignTimeDbContextFactory<EfContext>
{
    public EfContext CreateDbContext(string[] args)
    {
        EnvFile.Load();

        var optionsBuilder = new DbContextOptionsBuilder<EfContext>();
        optionsBuilder.UseNpgsql(DatabaseConfig.BuildConnectionString());

        return new EfContext(optionsBuilder.Options);
    }
}
