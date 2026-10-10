using System.ComponentModel.DataAnnotations;

namespace ExpenseWorkflow.Api.Models;

public sealed class ExpenseRequest
{
    public int Id { get; set; }
    [Required, MaxLength(20)] public string ExpenseType { get; set; } = "OPEX";
    [Required, MaxLength(120)] public string Title { get; set; } = "";
    [Required, MaxLength(80)] public string Department { get; set; } = "";
    [Required, MaxLength(100)] public string RequestedBy { get; set; } = "";
    [Required, MaxLength(500)] public string Justification { get; set; } = "";
    public decimal Amount { get; set; }
    [Required, MaxLength(3)] public string Currency { get; set; } = "PKR";
    [Required, MaxLength(30)] public string Status { get; set; } = "Draft";
    [MaxLength(300)] public string? DecisionComment { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<ExpenseHistory> History { get; set; } = new();
}
