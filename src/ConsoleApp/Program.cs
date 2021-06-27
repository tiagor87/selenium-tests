using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace SeleniumTests.ConsoleApp
{
    class Product
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public int Value { get; set; }

        public override string ToString()
        {
            return $"{{ \"name\": \"{Name}\", \"link\": \"{Link}\",\"value\": {Value} }}";
        }
    }
    
    class Program
    {
        static async Task Main(string[] args)
        {
            using var driver = new RemoteWebDriver(new Uri("http://localhost:4448"), new FirefoxOptions());
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
                driver.Navigate().GoToUrl("https://kabum.com.br");
                var searchInput = driver.FindElement(By.ClassName("sprocura"));
                searchInput.SendKeys("Rtx" + Keys.Enter);

                var productList = driver.FindElements(By.CssSelector("#listagem-produtos .item-nome"));
                var products = new List<Product>();
                foreach (var item in productList)
                {
                    var product = new Product();
                    product.Name = item.Text;
                    product.Link = item.GetAttribute("href");
                    products.Add(product);
                }

                foreach (var product in products)
                {
                    var done = false;
                    do
                    {
                        try
                        {
                            driver.Navigate().GoToUrl(product.Link);
                            var priceElement = driver.FindElement(By.CssSelector(".openboxbutton-wrapper"));
                            product.Value = int.Parse(priceElement.GetAttribute("data-preco").Replace(".", ""));
                            Console.WriteLine(product);
                            done = true;
                        }
                        catch
                        {
                            await Task.Delay(1000);
                        }
                    } while (!done);
                }
            }
            catch
            {
                
            }
            finally
            {
                driver.Close();
            }
        }
    }
}
