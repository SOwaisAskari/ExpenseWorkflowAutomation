using System.ComponentModel.DataAnnotations;

namespace ExpenseWorkflow.Api.Contracts;

public sealed class UpdateExpenseRequest
{
    [Required, RegularExpression("^(OPEX|CAPEX)$")] public string ExpenseType { get; init; } = "OPEX";
    [Required, StringLength(120, MinimumLength = 4)] public string Title { get; init; } = "";
    [Required, StringLength(80, MinimumLength = 2)] public string Department { get; init; } = "";
    [Required, StringLength(100, MinimumLength = 2)] public string RequestedBy { get; init; } = "";
    [Required, StringLength(500, MinimumLength = 10)] public string Justification { get; init; } = "";
    [Range(typeof(decimal), "1", "1000000000")] public decimal Amount { get; init; }
    [Required, RegularExpression("^[A-Z]{3}$")] public string Currency { get; init; } = "PKR";
}
