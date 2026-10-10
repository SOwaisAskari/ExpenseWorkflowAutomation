using ExpenseWorkflow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseWorkflow.Api.Data;

public sealed class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : DbContext(options)
{
    public DbSet<ExpenseRequest> Expenses => Set<ExpenseRequest>();
    public DbSet<ExpenseHistory> ExpenseHistory => Set<ExpenseHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseRequest>().HasIndex(x => x.Status);
        modelBuilder.Entity<ExpenseRequest>().Property(x => x.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<ExpenseHistory>().HasIndex(x => x.ExpenseRequestId);
    }
}
