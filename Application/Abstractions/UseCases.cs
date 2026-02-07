using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public sealed class AiCommandExecutor
    {
        private readonly IAppDbContext _db;

        public AiCommandExecutor(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<ExecutionResult> ExecuteAsync(ParsedAiCommand cmd, int currentYear, CancellationToken ct)
        {

            if (cmd.Intent == AiIntent.Unknown)
                return ExecutionResult.Fail("I couldn't determine what you want (create/update/delete). Try again.");

            if (string.IsNullOrWhiteSpace(cmd.CategoryName))
                return ExecutionResult.Fail("Category name is missing.");

            var categoryName = cmd.CategoryName.Trim();
            var year = cmd.Year ?? currentYear;

            switch (cmd.Intent)
            {
                case AiIntent.CreateCategory:
                    return await CreateCategoryAsync(categoryName, year, cmd, ct);

                case AiIntent.UpdateBudget:
                    return await UpdateBudgetAsync(categoryName, year, cmd, ct);

                case AiIntent.DeleteCategory:
                    return await DeleteCategoryAsync(categoryName, cmd, ct);

                default:
                    return ExecutionResult.Fail("Unsupported intent.");
            }
        }

        private async Task<ExecutionResult> CreateCategoryAsync(string name, int year, ParsedAiCommand cmd, CancellationToken ct)
        {
            var existing = await _db.FindCategoryByNameAsync(name, ct);
            if (existing is not null)
                return ExecutionResult.Fail($"Category '{name}' already exists.");

            var category = new Category(name);

            // If no amount => only create category
            if (cmd.TotalAmount is null && (cmd.MonthBudgets is null || cmd.MonthBudgets.Count == 0))
            {
                await _db.AddCategoryAsync(category, ct);
                await _db.SaveChangesAsync(ct);
                return ExecutionResult.Ok($"Created category '{name}'.");
            }

            // Must add category first so it gets an Id and is tracked
            await _db.AddCategoryAsync(category, ct);
            await _db.SaveChangesAsync(ct);

            // Now upsert budget entries via DbSet (reliable tracking)
            if (cmd.MonthBudgets is { Count: > 0 })
            {
                foreach (var kv in cmd.MonthBudgets)
                    await _db.UpsertBudgetEntryAsync(category.Id, year, kv.Key, kv.Value, ct);

                await _db.SaveChangesAsync(ct);
                return ExecutionResult.Ok($"Created '{name}' with month budgets for {year}.");
            }

            // Split total across months
            var months = cmd.Months is { Count: > 0 } ? cmd.Months : Enumerable.Range(1, 12).ToList();
            var total = cmd.TotalAmount ?? 0m;
            var allocations = BudgetMath.SplitEvenly(total, months);

            foreach (var kv in allocations)
                await _db.UpsertBudgetEntryAsync(category.Id, year, kv.Key, kv.Value, ct);

            await _db.SaveChangesAsync(ct);
            return ExecutionResult.Ok($"Created '{name}' and allocated {total} across {allocations.Count} month(s) in {year}.");
        }

        private async Task<ExecutionResult> UpdateBudgetAsync(string name, int year, ParsedAiCommand cmd, CancellationToken ct)
        {
            var category = await _db.FindCategoryByNameAsync(name, ct);
            if (category is null)
                return ExecutionResult.Fail($"Category '{name}' not found.");

            decimal? amount = null;

            //if (!string.IsNullOrWhiteSpace(cmd.AmountExpression))
            //    amount = AmountEvaluator.Evaluate(cmd.AmountExpression!);


            if (cmd.MonthBudgets is { Count: > 0 })
            {
                foreach (var kv in cmd.MonthBudgets)
                    await _db.UpsertBudgetEntryAsync(category.Id, year, kv.Key, kv.Value, ct);

                await _db.SaveChangesAsync(ct);
                return ExecutionResult.Ok($"Updated '{name}' month budget(s) for {year}.");
            }

            if (cmd.TotalAmount is not null)
            {
                var months = cmd.Months is { Count: > 0 } ? cmd.Months : Enumerable.Range(1, 12).ToList();
                var allocations = BudgetMath.SplitEvenly(cmd.TotalAmount.Value, months);

                foreach (var kv in allocations)
                    await _db.UpsertBudgetEntryAsync(category.Id, year, kv.Key, kv.Value, ct);

                await _db.SaveChangesAsync(ct);
                return ExecutionResult.Ok($"Updated '{name}' and allocated {cmd.TotalAmount.Value} across {allocations.Count} month(s) in {year}.");
            }

            return ExecutionResult.Fail("Update needs an amount and/or month (e.g., 'March to 200').");
        }

        private async Task<ExecutionResult> DeleteCategoryAsync(string name, ParsedAiCommand cmd, CancellationToken ct)
        {
            var category = await _db.FindCategoryByNameAsync(name, ct);
            if (category is null)
                return ExecutionResult.Fail($"Category '{name}' not found.");

            if (cmd.RequiresConfirmation)
            {
                return ExecutionResult.NeedsConfirmation(cmd.ConfirmationMessage
                    ?? $"Are you sure you want to delete category '{name}'?");
            }

            await _db.RemoveCategoryAsync(category, ct);
            await _db.SaveChangesAsync(ct);
            return ExecutionResult.Ok($"Deleted category '{name}'.");
        }
    }

    public sealed record ExecutionResult(bool Success, bool NeedsConfirmationd, string Message)
    {
        public static ExecutionResult Ok(string message) => new(true, false, message);
        public static ExecutionResult Fail(string message) => new(false, false, message);
        public static ExecutionResult NeedsConfirmation(string message) => new(false, true, message);
    }

}
