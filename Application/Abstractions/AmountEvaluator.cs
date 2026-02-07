using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public static class AmountEvaluator
    {
        public static decimal Evaluate(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr))
                throw new ArgumentException("Expression is empty.");

            // Only allow digits, operators, decimal points, whitespace, parentheses
            foreach (var ch in expr)
            {
                if (!(char.IsDigit(ch) || "+-*/(). ".Contains(ch)))
                    throw new InvalidOperationException("Invalid characters in expression.");
            }

            var e = new Expression(expr);
            var result = e.Evaluate();

            if (result is null) throw new InvalidOperationException("Expression returned null.");

            var dec = Convert.ToDecimal(result);
            if (dec < 0) throw new InvalidOperationException("Amount cannot be negative.");
            return dec;
        }
    }

}
