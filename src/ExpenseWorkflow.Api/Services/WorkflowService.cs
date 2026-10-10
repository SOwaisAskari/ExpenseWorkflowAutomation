using ExpenseWorkflow.Api.Contracts;
using ExpenseWorkflow.Api.Data;
using ExpenseWorkflow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseWorkflow.Api.Services;

public sealed class WorkflowService(WorkflowDbContext db)
{
    public async Task<ExpenseRequest> CreateAsync(CreateExpenseRequest input, CancellationToken ct)
    {
        var item = new ExpenseRequest
        {
            ExpenseType = input.ExpenseType, Title = input.Title.Trim(), Department = input.Department.Trim(),
            RequestedBy = input.RequestedBy.Trim(), Justification = input.Justification.Trim(),
            Amount = input.Amount, Currency = input.Currency.ToUpperInvariant(), Status = "Draft"
        };
        item.History.Add(new ExpenseHistory { FromStatus = "New", ToStatus = "Draft", Actor = item.RequestedBy, Comment = "Draft created." });
        db.Expenses.Add(item);
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<ExpenseRequest?> UpdateAsync(int id, UpdateExpenseRequest input, CancellationToken ct)
    {
        var item = await db.Expenses.FindAsync([id], ct);
        if (item is null) return null;
        if (item.Status is not ("Draft" or "Reverted"))
            throw new InvalidOperationException("Only Draft or Reverted requests can be edited.");
        item.ExpenseType = input.ExpenseType; item.Title = input.Title.Trim(); item.Department = input.Department.Trim();
        item.RequestedBy = input.RequestedBy.Trim(); item.Justification = input.Justification.Trim();
        item.Amount = input.Amount; item.Currency = input.Currency.ToUpperInvariant();
        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<ExpenseRequest?> SubmitAsync(int id, CancellationToken ct)
    {
        var item = await db.Expenses.Include(x => x.History).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return null;
        if (item.Status is not ("Draft" or "Reverted"))
            throw new InvalidOperationException("Only Draft or Reverted requests can be submitted.");
        var from = item.Status;
        item.Status = "PendingDepartment";
        item.DecisionComment = null;
        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        item.History.Add(new ExpenseHistory { FromStatus = from, ToStatus = item.Status, Actor = item.RequestedBy, Comment = "Submitted for department-head review." });
        await db.SaveChangesAsync(ct);
        return item;
    }

    public async Task<ExpenseRequest?> DecideAsync(int id, DecisionRequest input, CancellationToken ct)
    {
        var item = await db.Expenses.Include(x => x.History).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return null;

        string next;
        if (input.Actor == "DepartmentHead" && item.Status == "PendingDepartment")
            next = input.Decision == "Approve" ? "PendingFinance" : "Reverted";
        else if (input.Actor == "Finance" && item.Status == "PendingFinance")
            next = input.Decision == "Approve" ? "Approved" : "Reverted";
        else
            throw new InvalidOperationException($"Actor {input.Actor} cannot make a decision while request status is {item.Status}.");

        var previous = item.Status;
        item.Status = next;
        item.DecisionComment = input.Comment.Trim();
        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        item.History.Add(new ExpenseHistory
        {
            FromStatus = previous, ToStatus = next, Actor = input.Actor,
            Comment = $"{input.Decision}: {input.Comment.Trim()}"
        });
        await db.SaveChangesAsync(ct);
        return item;
    }
}
