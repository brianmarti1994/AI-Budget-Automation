using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public enum AiIntent
    {
        CreateCategory,
        UpdateBudget,
        DeleteCategory,
        Unknown
    }

    public sealed record ParsedAiCommand
    {
        public AiIntent Intent { get; init; } = AiIntent.Unknown;

        public string? CategoryName { get; init; }

        // If year missing -> default to currentYear (we’ll apply default in handler if null)
        public int? Year { get; init; }

        // TotalAmount is used mainly for Create; can be null
        public decimal? TotalAmount { get; init; }

        // Optional month allocations. If user says “March to 200” => MonthBudgets[3]=200
        // If user says “Jan,Feb,Mar divide 4000” => months list used by handler to split evenly.
        public List<int>? Months { get; init; } // list of month numbers 1..12
        public Dictionary<int, decimal>? MonthBudgets { get; init; } // month->amount (explicit)

        // Delete requires confirmation
        public bool RequiresConfirmation { get; init; }
        public string? ConfirmationMessage { get; init; }
    }
}
