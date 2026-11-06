using System;

namespace CafeSimulation.Models
{
    public class ComboItem : MenuItem
    {
        public MenuItem Drink { get; set; }
        public MenuItem Food { get; set; }

        public ComboItem(int id, string name, MenuItem drink, MenuItem food, decimal discountGrn)
            : base(id, name, (drink.Price + food.Price) - discountGrn, $"{drink.Prefix}-{food.Prefix}")
        {
            Drink = drink;
            Food = food;
        }

        public override string GetDescription()
        {
            return $"[{Id}] {Name,-22} - {Price} грн (СЕТ: {Drink.Name} + {Food.Name})";
        }
    }

}