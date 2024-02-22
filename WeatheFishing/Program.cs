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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WeatheFishing
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
           .AddLogging(builder =>
           {
               builder.AddConsole(); // Adds the console logger
           })
           .BuildServiceProvider();

            ILogger<WeatherService> logger = new Logger<WeatherService>(new LoggerFactory());

            Console.OutputEncoding = Encoding.UTF8;
            var urlCurrentWeather = "http://www.weather.bg/index.php?koiFail=tekushti&lng=0";
            var urlWeatherIn3Days = "http://www.meteo.bg/bg/forecast/gradove";
            var urlRiverWaterLevel = "http://www.meteo.bg/bg/rekiTablitsa";

            HttpClient httpClient = new HttpClient();
            HtmlWeb htmlWeb = new HtmlWeb();
            IWeatherService weatherService = new WeatherService(htmlWeb, logger);


            // escape the ""        
            for (int i = 0; i < CityConstants.CitiesForCurrentWeather.Length; i++)
            {
                CityConstants.CitiesForCurrentWeather[i] = CityConstants.CitiesForCurrentWeather[i].Replace("'", "''");
            }
         

            Task<List<string>> currentWeatherDataTask = weatherService.GetCurrentWeatherDataAsync(urlCurrentWeather);
            List<string> currentWeatherData = await currentWeatherDataTask;


            Task<List<string>> getWeatherDataFor3DaysAsync = weatherService.GetWeatherDataFor3DaysAsync(urlWeatherIn3Days);
            List<string> getWeatherDataFor3Days = await getWeatherDataFor3DaysAsync;


            Task<List<string>> getRiverWatherLevelAsync = weatherService.GetRiverWatherLevelAsync(urlRiverWaterLevel);
            List<string> getRiverWatherLevel = await getRiverWatherLevelAsync;

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

            if (getRiverWatherLevel != null)
            {
                Console.WriteLine("The water level in rivers \n");
                Console.WriteLine(string.Join(Environment.NewLine, getRiverWatherLevel));
            }

            httpClient.Dispose();
            serviceProvider.Dispose();
        }        

    }
}
