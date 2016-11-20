using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace DatabaseScraper
{
    static class Scraper
    {
        const int BUFFER_SIZE = 4096;

        private const string NAME_CATEGORY_INDEX = "NameCategoryIndex.txt";
        private const string CATEGORIES = "Categories.txt";

        private static Dictionary<string, string> nameToCategories = new Dictionary<string, string>();
        private static bool updateIndex = false;



        public static void Initialize()
        {
            if (!OnlineStorage.CheckFileExists(NAME_CATEGORY_INDEX))
            {
                return;
            }

            string contents = OnlineStorage.GetFile(NAME_CATEGORY_INDEX);
            string[] indicies = contents.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string index in indicies)
            {
                if (index.Length > 0)
                {
                    string[] parts = index.Split(':');
                    try
                    {
                        nameToCategories.Add(parts[0], parts[1]);
                    }
                    catch
                    {
                    }
                }
            }
        }



        public static List<MenuItem> FetchContents(List<string> urls)
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            IWebDriver driver = new ChromeDriver();

            foreach (string url in urls)
            {
                switch (url)
                {
                    case "http://www.pizzapizza.ca/view-our-menu/":
                        Gather_PizzaPizza(driver, menuItems, url);
                        break;
                    case "http://misterlaffa.com/menu.php#mix":
                        Gather_MrLaffa(driver, menuItems, url);
                        break;
                    case "https://order.bostonpizza.com/#/menu/9F5B31DD-44D0-4508-BA45-F428C7582E5F/ToGo/Pickup":
                        Gather_BostonPizza(driver, menuItems, url);
                        break;
                }
            }

            List<string> dict_values = new List<string>();
            foreach (KeyValuePair<string, string> entry in nameToCategories)
            {
                if (!dict_values.Contains(entry.Value))
                {
                    dict_values.Add(entry.Value);
                }
            }

            foreach (MenuItem item in menuItems)
            {
                if (!(nameToCategories.ContainsKey(item.Name)))
                {
                    updateIndex = true;
                    string category = PromptCategory(item.Name, dict_values);
                    nameToCategories.Add(item.Name, category);
                    item.SetCategory(category);
                }
                else
                {
                    item.SetCategory(nameToCategories[item.Name]);
                }
            }

            if (updateIndex)
            {
                string contents = "";
                foreach (MenuItem item in menuItems)
                {
                    string text = "";
                    text += item.Name;
                    text += ":";
                    text += item.Category;
                    text += "\n";

                    contents += text;
                }
                OnlineStorage.Overwrite(NAME_CATEGORY_INDEX, contents);

                contents = "";
                foreach (string value in dict_values)
                {
                    string text = "";
                    text += value;
                    text += "\n";
                    contents += text;
                }
                OnlineStorage.Overwrite(CATEGORIES, contents);
            }

            string[] categories = new string[dict_values.Count];
            foreach (MenuItem item in menuItems)
            {
                string category = item.Category;
                int index = dict_values.IndexOf(category);

                string temp = categories[index];
                string content = item.Franchise + ":" + item.Name + ":" + item.Price.ToString() + "\n";
                temp += content;
                categories[index] = temp;
            }
            for (int a = 0; a < categories.Length; a++)
            {
                string filename = dict_values[a] + ".txt";
                string contents = categories[a];
                OnlineStorage.Overwrite(filename, contents);
            }

            foreach (MenuItem item in menuItems)
            {
                //item.Print();
            }

            return menuItems;
        }

        private static string PromptCategory(string item, List<string> values)
        {
            string result = "";
            while (result == "")
            {
                Console.Write("Input category for \"" + item + "\": ");
                string input = Console.ReadLine();
                if (input == "/lc")
                {
                    foreach (string val in values)
                    {
                        Console.WriteLine(val);
                    }
                }
                else
                {
                    result = input;
                    if (!values.Contains(result))
                    {
                        values.Add(result);
                    }
                }
            }
            return result;
        }

        private static void Gather_BostonPizza(IWebDriver driver, List<MenuItem> menuItems, string url)
        {
            string franchise = "Boston Pizza";
            //driver.Url = url;

            string hrefPizza = "https://order.bostonpizza.com/#/menu/9F5B31DD-44D0-4508-BA45-F428C7582E5F/ToGo/Pickup/group/9aeeb4c5-0ff4-47e4-97a1-8f4bbb459dfd";
            driver.Url = hrefPizza;
            IReadOnlyCollection<IWebElement> namesPizzas_ = driver.FindElements(By.ClassName("title"));
            List<string> namesPizzas__ = GetCleanListFromReadOnly(namesPizzas_);
            List<string> namePizzas = new List<string>();
            for (int a = 0; a < namesPizzas__.Count; a++)
            {
                if ((a >= 5) && (a <= 9))
                {
                    namePizzas.Add(namesPizzas__[a]);
                }
                else if ((a >= 11) && (a <= 14))
                {
                    namePizzas.Add(namesPizzas__[a]);
                }
                else if ((a >= 19) && (a <= 22))
                {
                    namePizzas.Add(namesPizzas__[a]);
                }
                else if ((a == 16) || (a == 17) || (a == 24))
                {
                    namePizzas.Add(namesPizzas__[a]);
                }
            }
            IReadOnlyCollection<IWebElement> pricePizzas_ = driver.FindElements(By.ClassName("price"));
            List<string> pricePizzas = GetCleanListFromReadOnly(pricePizzas_);
            pricePizzas.RemoveAt(0);
            for (int a = 0; a < namePizzas.Count; a++)
            {
                string price = pricePizzas[a].Substring(pricePizzas[a].IndexOf("$") + 1);
                menuItems.Add(new MenuItem(namePizzas[a], float.Parse(price), franchise));
            }
        }

        private static void Gather_MrLaffa(IWebDriver driver, List<MenuItem> menuItems, string url)
        {
            string franchise = "Mister Laffa";
            driver.Url = url;

            IReadOnlyCollection<IWebElement> everything_ = driver.FindElements(By.ClassName("item-description"));
            List<string> everything = GetCleanListFromReadOnly(everything_);
            for (int a = 0; a < everything.Count; a++)
            {
                string text = everything[a];
                string[] parts = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                string name = parts[1];
                string price = parts[0].Substring(parts[0].IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }
        }

        private static void Gather_PizzaPizza(IWebDriver driver, List<MenuItem> menuItems, string url)
        {
            string franchise = "Pizza Pizza";
            driver.Url = url;

            IWebElement continuebutton = GetElementById(driver, "geolocation_continue");
            continuebutton.Click();
            System.Threading.Thread.Sleep(3000);

            IWebElement chicken = GetElementById(driver, "40000");
            chicken.Click();
            System.Threading.Thread.Sleep(3000);
            IReadOnlyCollection<IWebElement> pricesChicken_ = driver.FindElements(By.ClassName("item_pricem"));
            List<string> pricesChicken = GetCleanListFromReadOnly(pricesChicken_);
            for (int a = 0; a < pricesChicken.Count; a++)
            {
                string text = pricesChicken[a];
                string name = text.Substring(0, text.IndexOf(":"));
                string price = text.Substring(text.IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }

            IWebElement pastapizza = GetElementById(driver, "25100");
            pastapizza.Click();
            System.Threading.Thread.Sleep(3000);
            IReadOnlyCollection<IWebElement> namesPastaPizza_ = driver.FindElements(By.ClassName("title_and_tagline"));
            IReadOnlyCollection<IWebElement> pricesPastaPizza_ = driver.FindElements(By.ClassName("item_price"));
            List<string> namesPastaPizza = GetCleanListFromReadOnly(namesPastaPizza_);
            List<string> pricesPastaPizza = GetCleanListFromReadOnly(pricesPastaPizza_);
            for (int a = 0; a < namesPastaPizza.Count; a++)
            {
                string name = namesPastaPizza[a];
                string price = pricesPastaPizza[a].Substring(pricesPastaPizza[a].IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }

            IWebElement salads = GetElementById(driver, "24100");
            salads.Click();
            System.Threading.Thread.Sleep(3000);
            IReadOnlyCollection<IWebElement> namesSalads_ = driver.FindElements(By.ClassName("item_name"));
            IReadOnlyCollection<IWebElement> pricesSalads_ = driver.FindElements(By.ClassName("item_price"));
            List<string> namesSalads = GetCleanListFromReadOnly(namesSalads_);
            List<string> pricesSalads = GetCleanListFromReadOnly(pricesSalads_);
            for (int a = 0; a < namesSalads.Count; a++)
            {
                string name = namesSalads[a];
                string price = pricesSalads[a].Substring(pricesSalads[a].IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }

            IWebElement sandwiches = GetElementById(driver, "30100");
            sandwiches.Click();
            System.Threading.Thread.Sleep(3000);
            IReadOnlyCollection<IWebElement> pricesSandwiches_ = driver.FindElements(By.ClassName("item_pricem"));
            List<string> pricesSandwiches = GetCleanListFromReadOnly(pricesSandwiches_);
            for (int a = 0; a < pricesSandwiches.Count; a++)
            {
                string text = pricesSandwiches[a];
                string name = text.Substring(0, text.IndexOf(":"));
                string price = text.Substring(text.IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }

            IWebElement drinks = GetElementById(driver, "50011");
            drinks.Click();
            System.Threading.Thread.Sleep(3000);
            IReadOnlyCollection<IWebElement> pricesDrinks_ = driver.FindElements(By.ClassName("item_pricem"));
            List<string> pricesDrinks = GetCleanListFromReadOnly(pricesDrinks_);
            for (int a = 0; a < pricesDrinks.Count; a++)
            {
                string text = pricesDrinks[a];
                string name = text.Substring(0, text.IndexOf(":"));
                string price = text.Substring(text.IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }

            IWebElement sweets = GetElementById(driver, "60200");
            sweets.Click();
            System.Threading.Thread.Sleep(3000);
            IReadOnlyCollection<IWebElement> namesSweets_ = driver.FindElements(By.ClassName("title_and_tagline"));
            IReadOnlyCollection<IWebElement> pricesSweets_ = driver.FindElements(By.ClassName("item_price"));
            List<string> namesSweets = GetCleanListFromReadOnly(namesSweets_);
            List<string> pricesSweets = GetCleanListFromReadOnly(pricesSweets_);
            for (int a = 0; a < namesSweets.Count; a++)
            {
                string name = namesSweets[a];
                string price = pricesSweets[a].Substring(pricesSweets[a].IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }

            IWebElement bread = GetElementById(driver, "31100");
            bread.Click();
            System.Threading.Thread.Sleep(3000);
            IReadOnlyCollection<IWebElement> namesBread_ = driver.FindElements(By.ClassName("item_name"));
            IReadOnlyCollection<IWebElement> pricesBread_ = driver.FindElements(By.ClassName("item_price"));
            List<string> namesBread = GetCleanListFromReadOnly(namesBread_);
            List<string> pricesBread = GetCleanListFromReadOnly(pricesBread_);
            for (int a = 0; a < namesBread.Count; a++)
            {
                string name = namesBread[a];
                string price = pricesBread[a].Substring(pricesBread[a].IndexOf("$") + 1);
                menuItems.Add(new MenuItem(name, float.Parse(price), franchise));
            }
        }

        private static IWebElement GetElementById(IWebDriver driver, string id)
        {
            IWebElement element = null;
            try
            {
                //WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
                //wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));
                element = driver.FindElement(By.Id(id));
            }
            catch
            {
            }
            System.Threading.Thread.Sleep(3000);
            return element;
        }

        private static List<string> GetCleanListFromReadOnly(IReadOnlyCollection<IWebElement> collection)
        {
            List<string> collection_ = new List<string>();
            foreach (IWebElement element in collection)
            {
                string text = element.Text;
                if (text != "")
                {
                    collection_.Add(text);
                }
            }
            return collection_;
        }
    }
}
