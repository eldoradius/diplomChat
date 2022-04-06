using diplomChat.Entities;
using Microsoft.EntityFrameworkCore;

namespace diplomChat.Helpers;

public class DataContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_configuration.GetConnectionString("BadoonDB"));
    }
    
    public DbSet<User> Users { get; set; }
}