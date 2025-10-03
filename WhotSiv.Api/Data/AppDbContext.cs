namespace WhotSiv.Api.Data;

using Microsoft.EntityFrameworkCore;
using WhotSiv.Data.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();                                                                                                                                                                                            
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}