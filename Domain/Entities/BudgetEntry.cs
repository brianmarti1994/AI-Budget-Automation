using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BudgetEntry
    {
        public Guid Id { get; private set; }
        public Guid CategoryId { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public decimal Amount { get; private set; }

        private BudgetEntry() { } // EF

        public BudgetEntry(Guid categoryId, int year, int month, decimal amount)
        {
            Id = Guid.NewGuid();
            CategoryId = categoryId;
            Year = year;
            Month = month;
            SetAmount(amount);
        }

        public void SetAmount(decimal amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

            // Round to 2 decimal places to match DB precision (HasPrecision(18, 2))
            amount = Math.Round(amount, 2);

            if (Amount == amount) return;
            Amount = amount;
        }
    }
}
