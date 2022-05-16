using MessagingApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MessagingApp.Tests.Utilities;

public static class TestDatabase
{
    public static void AddInMemoryDatabase(this IServiceCollection services)
    {
        // Remove registered instance of DbContextOptions<AppDbContext>
        var registeredOptions = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
        if (registeredOptions != null) services.Remove(registeredOptions);

        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
    }
}