using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new ApplicationDbContext(
            optionsBuilder.Options,
            new DesignTimeRequestContext()
        );
    }
}

/// <summary>
/// Dummy implementation for EF Core design-time tools (migrations)
/// </summary>
internal class DesignTimeRequestContext : IRequestContext
{
    public Guid UserId => Guid.Empty;
    public Guid? OrganizationId => null;
    public string Email => string.Empty;
}