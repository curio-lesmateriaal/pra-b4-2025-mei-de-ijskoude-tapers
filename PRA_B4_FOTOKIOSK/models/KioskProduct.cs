using System;

namespace PRA_B4_FOTOKIOSK.models
{
    public class KioskProduct
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }

        public KioskProduct(string name, double price, string description)
        {
            Name = name;
            Price = price;
            Description = description;
        }

        // Voor dropdown en prijslijst
        public override string ToString()
        {
            return $"{Name} - €{Price:0.00} - {Description}";
        }

    }
}
