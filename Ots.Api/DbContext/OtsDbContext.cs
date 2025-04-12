using Microsoft.EntityFrameworkCore;
using Ots.Api.Domain;

namespace Ots.Api;

public class OtsDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }
    public DbSet<CustomerPhone> CustomerPhones { get; set; }

    
    public OtsDbContext(DbContextOptions<OtsDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OtsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
