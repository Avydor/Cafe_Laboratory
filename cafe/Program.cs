using cafe;
using cafe.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Реєструємо сервіс через інтерфейс у DI контейнері
builder.Services.AddSingleton<ICoffeeShopService, CoffeeShop>();

var app = builder.Build();

app.UseCors("AllowAll");

// 1. Вхід / Авторизація з установкой балансу та перевіркою адміна
app.MapPost("/api/auth/login", (ICoffeeShopService shop, LoginRequest req) =>
{
    var adminNames = new[] { "Admin", "admin", "Дмитро Кудрявцев", "Іван", "Иван" };
    bool isAdmin = adminNames.Contains(req.CustomerName.Trim(), StringComparer.OrdinalIgnoreCase);

    decimal initialBalance = isAdmin ? 999999m : req.InitialBalance;
    shop.SetUserSession(req.CustomerName, initialBalance, isAdmin);

    return Results.Ok(new
    {
        success = true,
        name = req.CustomerName,
        balance = shop.UserBalance,
        isAdmin = isAdmin
    });
});

// 2. Поповнення баланса
app.MapPost("/api/user/topup", (ICoffeeShopService shop, TopUpRequest req) =>
{
    if (req.Amount <= 0)
    {
        return Results.BadRequest(new { success = false, message = "Сума має бути більше 0!" });
    }

    shop.TopUpBalance(req.Amount);
    return Results.Ok(new
    {
        success = true,
        newBalance = shop.UserBalance,
        message = $"Баланс поповнено на {req.Amount} грн!"
    });
});

// 3. Меню і Граф рекомендацій
app.MapGet("/api/menu", (ICoffeeShopService shop) =>
{
    var menuWithRecs = shop.Menu.Select(item => new
    {
        item.Id,
        item.Name,
        item.Price,
        Category = item.GetType().Name.Replace("Item", ""),
        Recommendations = shop.RecommendationGraph.GetRecommendations(item.Name)
    });
    return Results.Ok(menuWithRecs);
});

// 4. Статус каси, баланс, черга та логи
app.MapGet("/api/status", (ICoffeeShopService shop) =>
{
    return Results.Ok(new
    {
        Revenue = shop.TotalRevenue,
        UserBalance = shop.UserBalance,
        CurrentUserName = shop.CurrentUserName,
        IsAdmin = shop.IsAdmin,
        ActiveOrdersCount = shop.OrderQueue.Count,
        TotalOrdersCount = shop.OrderHistory.Count,
        Queue = shop.OrderQueue.Select(o => new {
            Code = o.Code,
            CustomerName = o.Customer?.Name ?? "Клієнт",
            Item = new { name = o.Item?.Name ?? "Товар", price = o.FinalPrice },
            Status = o.Status
        }),
        Logs = shop.SystemLogs.TakeLast(15)
    });
});

// 5. Створення замовлення
app.MapPost("/api/orders", (ICoffeeShopService shop, CreateOrderRequest req) =>
{
    if (shop.CreateOrder(req.CustomerName, req.MenuItemId, out string error))
    {
        return Results.Ok(new { success = true, message = "Замовлення додано в чергу C#!" });
    }
    return Results.BadRequest(new { success = false, message = error });
});

// 6. Обробка замовлення з черги(FIFO)
app.MapPost("/api/orders/process", (ICoffeeShopService shop) =>
{
    var processed = shop.ProcessNextOrder();
    if (processed != null)
    {
        return Results.Ok(new
        {
            success = true,
            order = new
            {
                code = processed.Code,
                item = new { name = processed.Item?.Name ?? "Товар" }
            }
        });
    }
    return Results.BadRequest(new { success = false, message = "Черга C# порожня!" });
});

// 7. Пошук замовлень(Адмінка)
app.MapGet("/api/orders/search", (ICoffeeShopService shop, string query) =>
{
    if (string.IsNullOrWhiteSpace(query)) return Results.Ok(new object[0]);
    var results = shop.OrderHistory
        .Where(o => o.Customer != null && o.Customer.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
        .Select(o => new { o.Code, CustomerName = o.Customer.Name, ItemName = o.Item.Name, Price = o.FinalPrice, Status = o.Status })
        .ToList();
    return Results.Ok(results);
});

// 8. Авто-тести
app.MapPost("/api/tests/run", (ICoffeeShopService shop) =>
{
    shop.RunTests();
    return Results.Ok(new { success = true, message = "Усі C# авто-тести пройдено!" });
});

// 9. Зброс каси (Адмінка)
app.MapPost("/api/system/reset", (ICoffeeShopService shop) =>
{
    shop.ResetState();
    return Results.Ok(new { success = true });
});

app.Run("http://localhost:5000");

public record LoginRequest(string CustomerName, decimal InitialBalance);
public record TopUpRequest(decimal Amount);
public record CreateOrderRequest(string CustomerName, int MenuItemId);