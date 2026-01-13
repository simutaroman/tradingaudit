using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. DB Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Auth & Identity
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IEmailSender<ApplicationUser>, AzureEmailSender>();

// 3. Web API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// !! НОВИЙ OPEN API (Нативний) !!
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IBlobService, BlobService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error during applying migrations");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    // !! ГЕНЕРАЦІЯ JSON ДОКУМЕНТУ !!
    // Це створить ендпоінт /openapi/v1.json
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// 4. Blazor Hosting Files
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// 5. Identity Endpoints
app.MapGroup("/api/identity").MapIdentityApi<ApplicationUser>();

// 6. Fallback to Blazor
app.MapFallbackToFile("index.html");

app.Run();