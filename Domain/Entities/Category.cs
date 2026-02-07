using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;

        private readonly List<BudgetEntry> _budgets = new();
        public IReadOnlyCollection<BudgetEntry> Budgets => _budgets;

        private Category() { } // EF

        public Category(string name)
        {
            Id = Guid.NewGuid();
            Rename(name);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            Name = name.Trim();
        }

        public void SetBudget(int year, int month, decimal amount)
        {
            var existing = _budgets.FirstOrDefault(b => b.Year == year && b.Month == month);
            if (existing is null)
                _budgets.Add(new BudgetEntry(Id, year, month, amount));
            else
                existing.SetAmount(amount);
        }
    }

}
