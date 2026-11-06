using System;

namespace CafeSimulation.Models
{
    public class VipCustomer : Customer
    {
        public decimal Discount { get; set; } = 0.10m; // 10% додаткова VIP-знижка

        public VipCustomer(string name, decimal balance) : base(name, balance)
        {
        }

        public override decimal CalculatePrice(decimal originalPrice)
        {
            return originalPrice * (1 - Discount);
        }
    }
}