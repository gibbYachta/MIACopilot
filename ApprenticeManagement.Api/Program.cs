using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Data;
using ApprenticeManagement.Api.Endpoints;
using ApprenticeManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApprenticeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=apprentices.db"));

builder.Services.AddScoped<IApprenticeService, ApprenticeService>();
builder.Services.AddScoped<IWorkJournalService, WorkJournalService>();
builder.Services.AddScoped<IGradeService, GradeService>();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApprenticeDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map all endpoint groups
app.MapApprenticeEndpoints();
app.MapWorkJournalEndpoints();
app.MapGradeEndpoints();

app.Run();
