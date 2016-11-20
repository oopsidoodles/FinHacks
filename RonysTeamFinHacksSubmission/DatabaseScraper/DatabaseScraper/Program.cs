using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test");

            OnlineStorage.Initialize();
            Scraper.Initialize();
            //Scraper scraper = new Scraper(ScrapeSite.PizzaPizza);

            List<string> urls = new List<string>();
            //urls.Add("https://order.bostonpizza.com/#/menu/9F5B31DD-44D0-4508-BA45-F428C7582E5F/ToGo/Pickup");
            urls.Add("http://misterlaffa.com/menu.php#mix");
            urls.Add("http://www.pizzapizza.ca/view-our-menu/");
            List<MenuItem> menuItems = Scraper.FetchContents(urls);
            Console.WriteLine("Fetched menu items");
            MenuItem[] menuItems_ = RonysSortingAlgorithm.SortByPrice(menuItems.ToArray(), 0, menuItems.Count - 1);
            foreach (MenuItem item in menuItems_)
            {
                item.Print();
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
