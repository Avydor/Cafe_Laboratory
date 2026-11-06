using System;

namespace CafeSimulation.Models
{
    public enum MilkType
    {
        Super,
        SuperPuper,
        Mega
    }
    public class CoffeeItem : MenuItem
    {
        public MilkType MilkType { get; set; }

        public CoffeeItem(int id, string name, decimal price, string prefix, MilkType milkType)
            : base(id, name, price, prefix)
        {
            MilkType = milkType;
        }

        public override string GetDescription()
        {
            return $"[{Id}] {Name,-22} - {Price} грн (Молоко: {MilkType})";
        }
    }
}