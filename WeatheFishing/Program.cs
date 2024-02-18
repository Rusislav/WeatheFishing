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
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var urlCurrentWeather = "http://www.weather.bg/index.php?koiFail=tekushti&lng=0";
            var urlWeatherIn3Days = "http://www.meteo.bg/bg/forecast/gradove";
            var urlRiverWaterLevel = "http://www.meteo.bg/bg/rekiTablitsa";
            var urlForecastForBulgaria = "http://www.weather.bg/index.php?koiFail=bg&lng=0";


            //var chromeDriverPath = @"C:\Users\rusiv\Desktop\Project\WeatheFishing\chromedriver.exe";
            //IWebDriver driver = new ChromeDriver(chromeDriverPath);

            //// Navigate to the webpage
            //driver.Navigate().GoToUrl(urlCurrentWeather);
            //driver.Manage().Window.Maximize();

            //// Find the element by its XPath (i can use other locators like ID, class name, etc.)
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            //IWebElement element = wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.ClassName("linkOpenClose")))[0];

            //// Scroll into view using WebDriver's built-in method
            //Actions actions = new Actions(driver);
            //actions.MoveToElement(element);
            //actions.Perform();

            //// Click the element using JavaScript
            //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
 

            // escape the ""        
            for (int i = 0; i < CityConstants.CitiesForCurrentWeather.Length; i++)
            {
                CityConstants.CitiesForCurrentWeather[i] = CityConstants.CitiesForCurrentWeather[i].Replace("'", "''");
            }
          
            IWeatherService weatherService = new WeatherService();

            Task<List<string>> currentWeatherDataTask =  weatherService.GetCurrentWeatherDataAsync(urlCurrentWeather);
            List<string> currentWeatherData = await currentWeatherDataTask;

           
            Task<List<string>> getWeatherDataFor3DaysAsync = weatherService.GetWeatherDataFor3DaysAsync(urlWeatherIn3Days);
            List<string> getWeatherDataFor3Days = await getWeatherDataFor3DaysAsync;

            if (currentWeatherData != null)
            {
                Console.WriteLine("Current Weather. \n");
                Console.WriteLine(string.Join(Environment.NewLine, currentWeatherData));
                Console.WriteLine(Environment.NewLine);
            }
            if (getWeatherDataFor3Days != null)
            {
                Console.WriteLine("Weather in the next 3 days. \n");
              
                Console.WriteLine(string.Join(Environment.NewLine, getWeatherDataFor3Days));
            }


            // ще ми трябва данни за район за плевен там се намира река панега , монтана там се намира река огоста 
            //пловдив там се намира река марица , Марица 	Пазарджик ,ктрджали там се намира язовир кърджали 
        }
    }
}
