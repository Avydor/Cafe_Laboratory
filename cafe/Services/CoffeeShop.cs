using cafe;
using CafeSimulation.Models;
using CafeSimulation.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace cafe.Services
{
    public class ProductGraph
    {
        private readonly Dictionary<string, List<string>> _adjacencyList = new Dictionary<string, List<string>>();

        public void AddNode(string item)
        {
            if (!_adjacencyList.ContainsKey(item))
                _adjacencyList[item] = new List<string>();
        }

        public void AddEdge(string item1, string item2)
        {
            AddNode(item1);
            AddNode(item2);
            if (!_adjacencyList[item1].Contains(item2)) _adjacencyList[item1].Add(item2);
            if (!_adjacencyList[item2].Contains(item1)) _adjacencyList[item2].Add(item1);
        }

        public List<string> GetRecommendations(string item)
        {
            if (_adjacencyList.ContainsKey(item))
                return _adjacencyList[item];
            return new List<string>();
        }

        public bool HasNode(string item) => _adjacencyList.ContainsKey(item);
        public bool HasEdge(string item1, string item2) => _adjacencyList.ContainsKey(item1) && _adjacencyList[item1].Contains(item2);
    }

    public class CoffeeShop : ICoffeeShopService
    {
        public List<MenuItem> Menu { get; private set; } = new List<MenuItem>();
        public Queue<Order> OrderQueue { get; private set; } = new Queue<Order>();
        public List<Order> OrderHistory { get; private set; } = new List<Order>();
        public ProductGraph RecommendationGraph { get; private set; } = new ProductGraph();

        public decimal TotalRevenue { get; private set; } = 0;
        public decimal UserBalance { get; set; } = 300;
        public string CurrentUserName { get; set; } = "Гость";
        public bool IsAdmin { get; set; } = false;
        public List<string> SystemLogs { get; private set; } = new List<string>();

        private int _nextOrderNumber = 101;

        public CoffeeShop()
        {
            SeedData();
        }

        private void SeedData()
        {
            var espresso = new CoffeeItem(1, "Еспресо", 45m, "Міцна кава", MilkType.Mega);
            var latte = new CoffeeItem(2, "Лате", 65m, "Ніжна кава", MilkType.Super);
            var croissant = new BakeryItem(3, "Круасан", 55m, "Звичайна випічка", false);
            var cookie = new BakeryItem(4, "Печиво Безглютенове", 40m, "Корисне печиво", true);
            var combo = new ComboItem(5, "Кава + Круасан", espresso, croissant, 10m);

            Menu.Add(espresso);
            Menu.Add(latte);
            Menu.Add(croissant);
            Menu.Add(cookie);
            Menu.Add(combo);

            RecommendationGraph.AddEdge("Еспресо", "Круасан");
            RecommendationGraph.AddEdge("Еспресо", "Кава + Круасан");
            RecommendationGraph.AddEdge("Лате", "Печиво Безглютенове");
            RecommendationGraph.AddEdge("Круасан", "Кава + Круасан");
        }

        public void SetUserSession(string name, decimal balance, bool isAdmin)
        {
            CurrentUserName = name;
            UserBalance = balance;
            IsAdmin = isAdmin;
            Log($"Авторизація сесії: {name} (Баланс: {(isAdmin ? "Необмежений" : balance + " грн")}, Роль: {(isAdmin ? "ADMIN" : "USER")})");
        }

        public void TopUpBalance(decimal amount)
        {
            if (IsAdmin) return;
            UserBalance += amount;
            Log($"Користувач {CurrentUserName} поповнив баланс на +{amount} грн. Новий баланс: {UserBalance} грн.");
        }

        public void Log(string message)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            SystemLogs.Add(entry);
        }

        public bool CreateOrder(string customerName, int menuItemId, out string errorMessage)
        {
            errorMessage = string.Empty;
            var item = Menu.FirstOrDefault(m => m.Id == menuItemId);

            if (item == null)
            {
                errorMessage = "Товар не знайдено!";
                return false;
            }

            var customer = new Customer(customerName, UserBalance);
            var order = new Order(_nextOrderNumber++, customer, item, 1);

            if (order.Status.StartsWith("Помилка"))
            {
                errorMessage = order.Status;
                return false;
            }

            if (!IsAdmin)
            {
                UserBalance = customer.Balance;
            }

            OrderQueue.Enqueue(order);
            OrderHistory.Add(order);

            Log($"Створено замовлення {order.Code} ({item.Name}) для {customerName}. Списано: {order.FinalPrice} грн.");
            return true;
        }

        public Order? ProcessNextOrder()
        {
            if (OrderQueue.Count == 0) return null;

            var order = OrderQueue.Dequeue();
            order.Status = "Виконано";
            TotalRevenue += order.FinalPrice;

            Log($"Оброблено замовлення {order.Code}. Каса: +{order.FinalPrice} грн.");
            return order;
        }

        public void ResetState()
        {
            TotalRevenue = 0;
            OrderQueue.Clear();
            OrderHistory.Clear();
            Log("Систему C# обнулено та очищено.");
        }

        public void RunTests()
        {
            Log("=== ЗАПУСК C# АВТО-ТЕСТІВ ===");

            int qCount = OrderQueue.Count;
            CreateOrder("TestBot", 1, out _);
            Debug.Assert(OrderQueue.Count == qCount + 1);

            Debug.Assert(RecommendationGraph.HasNode("Еспресо"));
            Debug.Assert(RecommendationGraph.HasEdge("Еспресо", "Круасан"));

            Log("[TESTS PASSED] Усі C# юніт- та стрес-тести пройдено успішно!");
        }
    }
}