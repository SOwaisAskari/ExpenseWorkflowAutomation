# Expense Workflow Automation — Portfolio Demo

A standalone expense-request workflow for demonstrating OPEX/CAPEX request submission, department and finance approvals, reverts, resubmissions, audit history, and reporting.

**This is an independent demo.** It is not a copy of an employer system. It contains no company records, business rules, or confidential data.

## Stack
- .NET 8 ASP.NET Core Web API
- Entity Framework Core + SQLite
- Responsive HTML/CSS/JavaScript dashboard
- Swagger/OpenAPI

## Run
1. Install the .NET 8 SDK and Visual Studio 2022.
2. Open `ExpenseWorkflowAutomation.sln`.
3. Set `ExpenseWorkflow.Api` as startup project and run with F5.
4. Open `/` for the workflow dashboard or `/swagger` for API documentation.
5. The database `expense-workflow.db` is created on first run.

Terminal:
```bash
dotnet restore
dotnet run --project src/ExpenseWorkflow.Api
```

## Demo workflow
- Create a request with category, department, amount, and business justification.
- Submit it into the department-head queue.
- Department Head can approve or revert.
- Finance can approve or revert requests that have passed department review.
- Reverted requests can be edited and resubmitted.
- The audit timeline records each state transition.
- CAPEX and OPEX share the same illustrative demo workflow. Customize rules only to match a specific approved use case.

## API
- `GET /api/expenses`
- `GET /api/expenses/{id}`
- `POST /api/expenses`
- `POST /api/expenses/{id}/submit`
- `POST /api/expenses/{id}/decision`
- `PUT /api/expenses/{id}` — draft or reverted requests only
- `GET /api/expenses/{id}/history`
- `GET /api/dashboard/summary`

## Portfolio description
Built an independent expense workflow automation demo using .NET 8 Web API, EF Core, SQLite, and a responsive operations dashboard. Implemented request lifecycle tracking, department/finance decisions, revert-and-resubmit handling, audit history, and summary reporting.

## Safety notes
This is a learning/demo application, not a production financial approval system. It has no authentication, authorization, attachment scanning, or segregation-of-duties controls. Use fictional information only.
