using System.ComponentModel.DataAnnotations;

namespace ExpenseWorkflow.Api.Contracts;

public sealed class DecisionRequest
{
    [Required, RegularExpression("^(DepartmentHead|Finance)$")] public string Actor { get; init; } = "";
    [Required, RegularExpression("^(Approve|Revert)$")] public string Decision { get; init; } = "";
    [Required, StringLength(300, MinimumLength = 3)] public string Comment { get; init; } = "";
}
