using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScraper
{
    class MenuItem
    {
        public MenuItem(string name, float price, string franchise, string category = "")
        {
            Name = name;
            Price = price;
            Franchise = franchise;
        }



        public string Name { private set; get; }
        public float Price { private set; get; }
        public string Franchise { private set; get; }
        public string Category { private set; get; }



        public void SetCategory(string category)
        {
            Category = category;
        }

        public void Print()
        {
            Console.WriteLine(String.Format("{0} - {1}   {2}: ${3}", Franchise, Category, Name, Price));
        }
    }
}
