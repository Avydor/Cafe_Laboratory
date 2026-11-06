using System;

namespace CafeSimulation.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public Customer(string name, decimal balance)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Гість" : name;
            Balance = balance < 0 ? 0 : balance;
        }

        public virtual decimal CalculatePrice(decimal originalPrice)
        {
            return originalPrice;
        }
    }

}