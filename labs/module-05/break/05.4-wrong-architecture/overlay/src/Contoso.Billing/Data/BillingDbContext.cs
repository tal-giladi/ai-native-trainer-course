using Contoso.Billing.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Contoso.Billing.Data;

// BILL-150 plan, Approach 2: "introduce EF Core for writes; the repositories have no write pattern".
public sealed class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<InvoiceReminder> InvoiceReminders => Set<InvoiceReminder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvoiceReminder>(e =>
        {
            e.ToTable("InvoiceReminder", "dbo");
            e.HasKey(r => r.InvoiceReminderId);
            e.Property(r => r.SentUtc).HasColumnType("datetimeoffset(0)");
        });
    }
}
