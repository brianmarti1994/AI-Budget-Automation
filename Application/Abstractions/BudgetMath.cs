using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public static class BudgetMath
    {
        public static Dictionary<int, decimal> SplitEvenly(decimal total, IEnumerable<int> months)
        {
            var list = months.Distinct().OrderBy(x => x).ToList();
            if (list.Count == 0) list = Enumerable.Range(1, 12).ToList();

            var per = Math.Round(total / list.Count, 2);
            var map = list.ToDictionary(m => m, _ => per);

            // Fix rounding diff by adding/subtracting to last month
            var sum = map.Values.Sum();
            var diff = total - sum;
            if (diff != 0)
            {
                var last = list.Last();
                map[last] = map[last] + diff;
            }
            return map;
        }
    }
}
