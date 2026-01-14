using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace Context;

public class EfContextFactory : IDesignTimeDbContextFactory<EfContext>
{
    public EfContext CreateDbContext(string[] args)
    {
        var currentDir = Directory.GetCurrentDirectory();
        var parentDir = Directory.GetParent(currentDir)?.FullName;
        
        // Look for .env in common locations
        var possiblePaths = new[]
        {
            Path.Combine(parentDir ?? "", "API", ".env"),        
        };
        

        foreach (var path in possiblePaths)
        {
            if (!File.Exists(path)) continue;
            Env.Load(path);
            break;
        }
        
        var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
        

        var optionsBuilder = new DbContextOptionsBuilder<EfContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new EfContext(optionsBuilder.Options);
    }
}