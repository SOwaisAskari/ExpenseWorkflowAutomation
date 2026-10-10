using ExpenseWorkflow.Api.Data;
using ExpenseWorkflow.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<WorkflowService>();
builder.Services.AddDbContext<WorkflowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Workflow") ?? "Data Source=expense-workflow.db"));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    db.Database.EnsureCreated();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", application = "Expense Workflow Automation Demo" }));
app.MapFallbackToFile("index.html");
app.Run();

public partial class Program;
