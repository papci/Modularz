using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modularz.Data.EF;

public class DesignTimeFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<BlogDbContext>();
        builder.UseNpgsql(
            "Server=(localdb)\\mssqllocaldb;Database=ihmigrations;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new BlogDbContext(builder.Options);
    }
}