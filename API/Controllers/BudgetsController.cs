using API.Contracts;
using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/budgets")]
    public class BudgetsController : ControllerBase
    {
        [HttpGet("pivot")]
        public async Task<ActionResult<List<BudgetPivotRowDto>>> GetPivot(
            [FromQuery] int? year,
            [FromServices] IAppDbContext db,
            CancellationToken ct)
        {
            var y = year ?? DateTime.UtcNow.Year;

            // Load minimal data (categories + their budgets for requested year)
            var categories = await db.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Budgets = c.Budgets.Where(b => b.Year == y)
                        .Select(b => new { b.Month, b.Amount })
                        .ToList()
                })
                .ToListAsync(ct);

            static decimal M(IEnumerable<dynamic> budgets, int month)
                => budgets.Where(b => (int)b.Month == month).Sum(b => (decimal)b.Amount);

            var result = categories.Select(c =>
            {
                var jan = M(c.Budgets, 1);
                var feb = M(c.Budgets, 2);
                var mar = M(c.Budgets, 3);
                var apr = M(c.Budgets, 4);
                var may = M(c.Budgets, 5);
                var jun = M(c.Budgets, 6);
                var jul = M(c.Budgets, 7);
                var aug = M(c.Budgets, 8);
                var sep = M(c.Budgets, 9);
                var oct = M(c.Budgets, 10);
                var nov = M(c.Budgets, 11);
                var dec = M(c.Budgets, 12);

                var total = jan + feb + mar + apr + may + jun + jul + aug + sep + oct + nov + dec;

                return new BudgetPivotRowDto(
                    c.Id, c.Name,
                    jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec,
                    total);
            }).ToList();

            return Ok(result);
        }
    }
}
