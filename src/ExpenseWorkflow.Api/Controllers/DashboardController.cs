using ExpenseWorkflow.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseWorkflow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(WorkflowDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken ct)
    {
        var items = await db.Expenses.AsNoTracking().ToListAsync(ct);
        return Ok(new
        {
            total = items.Count,
            awaitingDepartment = items.Count(x => x.Status == "PendingDepartment"),
            awaitingFinance = items.Count(x => x.Status == "PendingFinance"),
            approved = items.Count(x => x.Status == "Approved"),
            reverted = items.Count(x => x.Status == "Reverted"),
            totalRequested = items.Sum(x => x.Amount)
        });
    }
}
