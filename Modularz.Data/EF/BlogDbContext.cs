using Microsoft.EntityFrameworkCore;

namespace Modularz.Data.EF;

public class BlogDbContext : DbContext
{
    protected string Connectionstring;

    public BlogDbContext()
    {
    }

    public BlogDbContext(string connectionString)
    {
        this.Connectionstring = connectionString;
    }

    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!string.IsNullOrWhiteSpace(Connectionstring))
            optionsBuilder.UseNpgsql(Connectionstring);
    }

    public DbSet<BlogUser> BlogUsers { get; set; }
    public DbSet<BlogPost> Posts { get; set; }

    public  bool IsInitialized()
    {
        return BlogUsers.Any(x => x.Rank == BlogUser.UserRank.Admin);
    }
}