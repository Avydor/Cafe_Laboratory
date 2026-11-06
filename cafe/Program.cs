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

builder.Services.AddSingleton<CoffeeShop>();

var app = builder.Build();
app.UseCors("AllowAll");

// 1. Вход / Авторизация с установкой начального баланса и проверкой админа
app.MapPost("/api/auth/login", (CoffeeShop shop, LoginRequest req) =>
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

// 2. Пополнение баланса
app.MapPost("/api/user/topup", (CoffeeShop shop, TopUpRequest req) =>
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

// 3. Меню и Граф рекомендаций
app.MapGet("/api/menu", (CoffeeShop shop) =>
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

// 4. Статус кассы, баланс, очередь и логи
app.MapGet("/api/status", (CoffeeShop shop) =>
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

// 5. Создание заказа
app.MapPost("/api/orders", (CoffeeShop shop, CreateOrderRequest req) =>
{
    if (shop.CreateOrder(req.CustomerName, req.MenuItemId, out string error))
    {
        return Results.Ok(new { success = true, message = "Замовлення додано в чергу C#!" });
    }
    return Results.BadRequest(new { success = false, message = error });
});

// 6. Обработка заказа из очереди (FIFO)
app.MapPost("/api/orders/process", (CoffeeShop shop) =>
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

// 7. Поиск заказов (Админка)
app.MapGet("/api/orders/search", (CoffeeShop shop, string query) =>
{
    if (string.IsNullOrWhiteSpace(query)) return Results.Ok(new object[0]);
    var results = shop.OrderHistory
        .Where(o => o.Customer != null && o.Customer.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
        .Select(o => new { o.Code, CustomerName = o.Customer.Name, ItemName = o.Item.Name, Price = o.FinalPrice, Status = o.Status })
        .ToList();
    return Results.Ok(results);
});

// 8. Авто-тесты
app.MapPost("/api/tests/run", (CoffeeShop shop) =>
{
    shop.RunTests();
    return Results.Ok(new { success = true, message = "Усі C# авто-тести пройдено!" });
});

// 9. Сброс кассы (Админка)
app.MapPost("/api/system/reset", (CoffeeShop shop) =>
{
    shop.ResetState();
    return Results.Ok(new { success = true });
});

app.Run("http://localhost:5000");

public record LoginRequest(string CustomerName, decimal InitialBalance);
public record TopUpRequest(decimal Amount);
public record CreateOrderRequest(string CustomerName, int MenuItemId);