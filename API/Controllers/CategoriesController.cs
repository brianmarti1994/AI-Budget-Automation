using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get([FromServices] IAppDbContext db, CancellationToken ct)
        {
            var items = await db.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Budgets = c.Budgets
                        .OrderBy(b => b.Year).ThenBy(b => b.Month)
                        .Select(b => new { b.Year, b.Month, b.Amount })
                })
                .ToListAsync(ct);

            return Ok(items);
        }
    }
}
