using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IAppDbContext
    {
        IQueryable<Category> Categories { get; }
        Task AddCategoryAsync(Category category, CancellationToken ct);
        Task<Category?> FindCategoryByNameAsync(string name, CancellationToken ct);
        Task<Category?> FindCategoryByIdAsync(Guid id, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task RemoveCategoryAsync(Category category, CancellationToken ct);

        /// <summary>
        /// Upsert a budget entry for a tracked category.
        /// If a BudgetEntry for (categoryId, year, month) exists, update its amount.
        /// If not, insert a new one.
        /// </summary>
        Task UpsertBudgetEntryAsync(Guid categoryId, int year, int month, decimal amount, CancellationToken ct);
    }

    public interface IAiCommandParser
    {
        Task<ParsedAiCommand> ParseAsync(string userPrompt, int currentYear, CancellationToken ct);
    }
}
