using System;

namespace CafeSimulation.Models
{
         public class MenuItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Prefix { get; set; }

            public MenuItem(int id, string name, decimal price, string prefix)
            {
                Id = id;
                Name = name;
                Price = price;
                Prefix = prefix;
            }

            public virtual string GetDescription()
            {
                return $"[{Id}] {Name,-22} - {Price} грн";
            }
        }

    
}