# Architecture

```text
Browser dashboard / API client
             |
             v
    ASP.NET Core Web API
   / ExpensesController \
  / DashboardController  \
             |
             v
       WorkflowService
             |
             v
      EF Core / SQLite
       |             |
 ExpenseRequest   ExpenseHistory
```

## Workflow states
- `Draft` → request saved but not submitted.
- `PendingDepartment` → awaiting department-head decision.
- `PendingFinance` → department approved; awaiting finance decision.
- `Approved` → both illustrative review stages approved.
- `Reverted` → requester must update and resubmit.

## Rules
- Only Draft and Reverted requests can be edited or submitted.
- DepartmentHead may decide only when status is PendingDepartment.
- Finance may decide only when status is PendingFinance.
- Every submission and decision creates a history record.
- Revert decisions require a comment; approval decisions also record a comment.

## Deliberate limitations
This sample uses a single demo workflow for OPEX and CAPEX to keep the project understandable. It has no identity management, role authorization, document upload, malware scanning, notification service, or production approval controls. Do not use as-is for real company expenses.
