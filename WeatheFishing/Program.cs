using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Text;
using WeatheFishing.Services;
using WeatheFishing.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WeatheFishing.Constants;

namespace WeatheFishing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var urlCurrentWeather = "http://www.weather.bg/index.php?koiFail=tekushti&lng=0";
            var urlWeatherIn3Days = "http://www.meteo.bg/bg/forecast/gradove";
            var urlRiverWaterLevel = "http://www.meteo.bg/bg/rekiTablitsa";
            var urlForecastForBulgaria = "http://www.weather.bg/index.php?koiFail=bg&lng=0";


            var chromeDriverPath = @"C:\Users\rusiv\Desktop\Project\WeatheFishing\chromedriver.exe";
            IWebDriver driver = new ChromeDriver(chromeDriverPath);

            // Navigate to the webpage
            driver.Navigate().GoToUrl(urlCurrentWeather);
            driver.Manage().Window.Maximize();

            // Find the element by its XPath (you can use other locators like ID, class name, etc.)
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            IWebElement element = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("linkOpenClose")))[0];

            // Scroll into view using WebDriver's built-in method
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();

            // Click the element using JavaScript
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
 

            // escape the ""        
            for (int i = 0; i < CityConstants.Cities.Length; i++)
            {
                CityConstants.Cities[i] = CityConstants.Cities[i].Replace("'", "''");
            }
          
            IWeatherService weatherService = new WeatherService();
            List<string> currentWeatherData = weatherService.GetCurrentWeatherData(urlCurrentWeather);

            if (currentWeatherData != null)
            {
                Console.WriteLine(string.Join(Environment.NewLine, currentWeatherData));
            }


            // ще ми трябва данни за район за плевен там се намира река панега , монтана там се намира река огоста 
            //пловдив там се намира река марица , Марица 	Пазарджик ,ктрджали там се намира язовир кърджали 
        }
    }
}
