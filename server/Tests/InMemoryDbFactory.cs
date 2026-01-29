using Context;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public static class InMemoryDbFactory
{
    public static EfContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EfContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        return new EfContext(options);
    }
}