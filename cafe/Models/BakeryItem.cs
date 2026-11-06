using System;

namespace CafeSimulation.Models
{
    public class BakeryItem : MenuItem
    {
        public bool IsGlutenFree { get; set; }

        public BakeryItem(int id, string name, decimal price, string prefix, bool isGlutenFree)
            : base(id, name, price, prefix)
        {
            IsGlutenFree = isGlutenFree;
        }

        public override string GetDescription()
        {
            string gf = IsGlutenFree ? "Без глютену" : "Звичайне";
            return $"[{Id}] {Name,-22} - {Price} грн ({gf})";
        }
    }

}