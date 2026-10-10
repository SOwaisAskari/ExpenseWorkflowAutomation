using ExpenseWorkflow.Api.Contracts;
using ExpenseWorkflow.Api.Data;
using ExpenseWorkflow.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseWorkflow.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public sealed class ExpensesController(WorkflowDbContext db, WorkflowService workflow) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] string? type, CancellationToken ct)
    {
        var query = db.Expenses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(type)) query = query.Where(x => x.ExpenseType == type);
        return Ok(await query.OrderByDescending(x => x.CreatedAtUtc).Select(x => new
        {
            x.Id, x.ExpenseType, x.Title, x.Department, x.RequestedBy, x.Amount, x.Currency,
            x.Status, x.DecisionComment, x.CreatedAtUtc, x.UpdatedAtUtc
        }).ToListAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await db.Expenses.AsNoTracking().Include(x => x.History).FirstOrDefaultAsync(x => x.Id == id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseRequest input, CancellationToken ct)
    {
        var item = await workflow.CreateAsync(input, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseRequest input, CancellationToken ct)
    {
        try
        {
            var item = await workflow.UpdateAsync(id, input, ct);
            return item is null ? NotFound() : Ok(item);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/submit")]
    public async Task<IActionResult> Submit(int id, CancellationToken ct)
    {
        try
        {
            var item = await workflow.SubmitAsync(id, ct);
            return item is null ? NotFound() : Ok(item);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/decision")]
    public async Task<IActionResult> Decide(int id, DecisionRequest input, CancellationToken ct)
    {
        try
        {
            var item = await workflow.DecideAsync(id, input, ct);
            return item is null ? NotFound() : Ok(item);
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpGet("{id:int}/history")]
    public async Task<IActionResult> History(int id, CancellationToken ct)
    {
        var exists = await db.Expenses.AnyAsync(x => x.Id == id, ct);
        if (!exists) return NotFound();
        return Ok(await db.ExpenseHistory.Where(x => x.ExpenseRequestId == id).OrderBy(x => x.CreatedAtUtc).ToListAsync(ct));
    }
}
