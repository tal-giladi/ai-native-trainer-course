using Microsoft.EntityFrameworkCore;

namespace Contoso.Billing.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Order> Orders => Set<Order>();   // mapped for invoice joins only
    }
}
