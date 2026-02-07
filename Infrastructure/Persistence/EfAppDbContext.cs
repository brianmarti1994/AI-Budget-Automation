using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public sealed class EfAppDbContext : IAppDbContext
    {
        private readonly AppDbContext _db;
        public EfAppDbContext(AppDbContext db) => _db = db;

        public IQueryable<Category> Categories =>
            _db.Categories.Include(c => c.Budgets);

        public Task AddCategoryAsync(Category category, CancellationToken ct) =>
            _db.Categories.AddAsync(category, ct).AsTask();

        public Task<Category?> FindCategoryByNameAsync(string name, CancellationToken ct) =>
           _db.Categories
              .Include(c => c.Budgets)
              .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), ct);

        public Task<Category?> FindCategoryByIdAsync(Guid id, CancellationToken ct) =>
            _db.Categories.Include(c => c.Budgets).FirstOrDefaultAsync(c => c.Id == id, ct);

        public Task RemoveCategoryAsync(Category category, CancellationToken ct)
        {
            _db.Categories.Remove(category);
            return Task.CompletedTask;
        }

        public async Task UpsertBudgetEntryAsync(Guid categoryId, int year, int month, decimal amount, CancellationToken ct)
        {
            amount = Math.Round(amount, 2);

            var existing = await _db.BudgetEntries
                .FirstOrDefaultAsync(b => b.CategoryId == categoryId && b.Year == year && b.Month == month, ct);

            if (existing is not null)
            {
                // Directly update via EF-tracked entity from DbSet query (not backing field)
                existing.SetAmount(amount);
            }
            else
            {
                // Insert new - EF fully tracks this because we go through DbSet.Add
                var entry = new BudgetEntry(categoryId, year, month, amount);
                _db.BudgetEntries.Add(entry);
            }
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }

        public Task AddBudgetEntryAsync(BudgetEntry entry, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
