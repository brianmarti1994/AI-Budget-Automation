using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<BudgetEntry> BudgetEntries => Set<BudgetEntry>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Category>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();

                // Map the relationship using the PROPERTY
                e.HasMany(x => x.Budgets)
                 .WithOne()
                 .HasForeignKey(be => be.CategoryId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Tell EF to populate the backing field
                e.Navigation(x => x.Budgets)
                 .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            b.Entity<BudgetEntry>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Amount).HasPrecision(18, 2);
                e.HasIndex(x => new { x.CategoryId, x.Year, x.Month }).IsUnique();
            });
        }
    }
}
