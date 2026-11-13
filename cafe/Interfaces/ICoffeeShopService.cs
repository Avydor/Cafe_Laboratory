using CafeSimulation.Models;
using CafeSimulation.Services;
using System.Collections.Generic;

namespace cafe.Services
{
    public interface ICoffeeShopService
    {
        List<MenuItem> Menu { get; }
        Queue<Order> OrderQueue { get; }
        List<Order> OrderHistory { get; }
        ProductGraph RecommendationGraph { get; }

        decimal TotalRevenue { get; }
        decimal UserBalance { get; set; }
        string CurrentUserName { get; set; }
        bool IsAdmin { get; set; }
        List<string> SystemLogs { get; }

        void SetUserSession(string name, decimal balance, bool isAdmin);
        void TopUpBalance(decimal amount);
        bool CreateOrder(string customerName, int menuItemId, out string errorMessage);
        Order? ProcessNextOrder();
        void ResetState();
        void RunTests();
    }
}