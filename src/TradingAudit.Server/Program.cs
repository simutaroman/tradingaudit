using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Reflection;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Server.Services;
using TradingAudit.Server.Services.Interfaces;
using TradingAudit.Shared.Constants;
using TradingAudit.Shared.Enums;

var builder = WebApplication.CreateBuilder(args);

// 1. DB Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Auth & Identity
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>(); ;

// 1. Створюємо глобальний конфіг
var config = TypeAdapterConfig.GlobalSettings;

// 2. Скануємо збірку на наявність класів, що реалізують IRegister (наш StrategyMappingConfig)
config.Scan(Assembly.GetExecutingAssembly());

// 3. Реєструємо сервіс мапера (Singleton, бо конфіг не змінюється)
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddAuthorization(options =>
{
    // 1. Політика: Хто може завантажувати картинки?
    // Логіка: Рівень >= Standard АБО це Адмін. (Support тільки читає, тому ні)
    options.AddPolicy(PolicyNames.CanUploadImages, policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            // Якщо Адмін - можна
            if (user.IsInRole("Admin")) return true;

            // Шукаємо клейм підписки
            var tierClaim = user.FindFirst("subscription_tier");
            if (tierClaim != null && int.TryParse(tierClaim.Value, out int tierValue))
            {
                // Перевіряємо чи рівень >= Standard (1)
                return tierValue >= (int)SubscriptionTier.Standard;
            }

            return false;
        }));

    // 2. Політика: Хто може експортувати в Excel?
    // Логіка: Рівень >= Pro АБО Адмін АБО Сапорт (їм треба для звітів)
    options.AddPolicy(PolicyNames.CanExportData, policy =>
        policy.RequireAssertion(context =>
        {
            if (context.User.IsInRole("Admin") || context.User.IsInRole("Support")) return true;

            var tierClaim = context.User.FindFirst("subscription_tier");
            if (tierClaim != null && int.TryParse(tierClaim.Value, out int tierValue))
            {
                return tierValue >= (int)SubscriptionTier.Pro;
            }
            return false;
        }));

    // 3. Політика: Хто має право на ЗАПИС (Create/Update/Delete)?
    // Логіка: Всі крім Support.
    options.AddPolicy(PolicyNames.RequireWriteAccess, policy =>
        policy.RequireAssertion(context =>
            !context.User.IsInRole("Support") // Сапорту заборонено
        ));
});

builder.Services.AddTransient<IEmailSender<ApplicationUser>, AzureEmailSender>();

// 3. Web API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// !! НОВИЙ OPEN API (Нативний) !!
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddScoped<IStrategyService, StrategyService>();

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

// === ПОЧАТОК БЛОКУ SEEDING ===
// Створюємо Scope, щоб отримати доступ до сервісів БД
// todo: refactor
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Викликаємо наш ініціалізатор
        // Треба додати using TradingAudit.Server.Data;
        await DbInitializer.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
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