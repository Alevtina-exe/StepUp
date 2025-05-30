using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightTracker.Models
{
    public class FoodProduct
    {
        public string? Barcode { get; set; }
        public string? Name { get; set; }
        public string? Brands { get; set; }
        public int Cal100g { get; set; }
        public double Protein100g { get; set; }
        public double Fat100g { get; set; }
        public double Carbon100g { get; set; }
        public string? Kcal { 
            get 
            { 
                if (Cal100g == 0) return "";
                return Cal100g.ToString() + " ккал"; 
            } 
        }
        public string NameForSearch
        {
            get
            {
                if (Name != null) return Name;
                return "Продукт не найден";
            }
        }
        public string? Serving { get; set; } = null;
        public double Amount { get; set; }
        public string CurrentCalories => ((int)(Amount * Cal100g / 100)).ToString() + " ккал";
        public string? ImageUrl { get; set; }
    }

}
