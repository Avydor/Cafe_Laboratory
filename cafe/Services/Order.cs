using System;
using System.Collections.Generic;
using System.Linq;
using CafeSimulation.Models; 

namespace CafeSimulation.Services
{
    public class Order
    {
        public string Code { get; set; }
        public Customer Customer { get; set; }
        public MenuItem Item { get; set; }
        public int Quantity { get; set; }
        public decimal FinalPrice { get; set; }
        public string Status { get; set; }

        public Order(int number, Customer customer, MenuItem item, int quantity = 1)
        {
            Code = $"{item.Prefix}-{number}";
            Customer = customer;
            Item = item;
            Quantity = quantity < 1 ? 1 : quantity;

            decimal basePrice = item.Price * Quantity;
            FinalPrice = customer.CalculatePrice(basePrice);

            if (customer.Balance >= FinalPrice)
            {
                customer.Balance -= FinalPrice;
                Status = "У черзі";
            }
            else
            {
                Status = "Помилка: Мало коштів";
            }
        }
    }
}