using System.ComponentModel.DataAnnotations;

namespace ExpenseWorkflow.Api.Models;

public sealed class ExpenseHistory
{
    public int Id { get; set; }
    public int ExpenseRequestId { get; set; }
    public ExpenseRequest? ExpenseRequest { get; set; }
    [Required, MaxLength(30)] public string FromStatus { get; set; } = "";
    [Required, MaxLength(30)] public string ToStatus { get; set; } = "";
    [Required, MaxLength(80)] public string Actor { get; set; } = "";
    [Required, MaxLength(300)] public string Comment { get; set; } = "";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
