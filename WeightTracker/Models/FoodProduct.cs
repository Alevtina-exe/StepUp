using Microsoft.Maui.Graphics.Text;
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
        public double Cal100g { get; set; }
        public double Protein100g { get; set; }
        public double Fat100g { get; set; }
        public double SaturatedFat100g { get; set; }
        public double Carbon100g { get; set; }
        public double Sugars100g { get; set; }
        public double ServingQuantity {  get; set; }
        public string? NutriscoreGrade { get; set; }
        public string? NovaGroup { get; set; }
        public string? Kcal { 
            get 
            {
                return Math.Round(Cal100g, 1).ToString() + " ккал"; 
            } 
        }
        public string NameForSearch
        {
            get
            {
                if (Name != null)
                {
                    if (Name.Length > 24) return Name.Substring(0, 21) + "...";
                    return Name;
                }
                return "Продукт не найден";
            }
        }
        public string? Serving { get; set; } = null;
        public double Amount { get; set; }
        public string CurrentCalories => ((int)(Amount * Cal100g / 100)).ToString() + " ккал";
        public string? ImageUrl { get; set; }
    }

}
