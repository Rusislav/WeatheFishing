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
           
                // Setup dependencie
                IServiceProvider serviceProvider = SetupDependencies();

                // Retrieve necessary services
                var logger = serviceProvider.GetService<ILogger<WeatherService>>();
                var htmlWeb = serviceProvider.GetService<HtmlWeb>();
                var weatherService = serviceProvider.GetService<IWeatherService>();

                // Fetch data asynchronously
                var currentWeatherData = await weatherService.GetCurrentWeatherDataAsync(CityConstants.UrlCurrentWeatherData);
                var weatherDataFor3Days = await weatherService.GetWeatherDataFor3DaysAsync(CityConstants.UrlWeatherDataFor3Day);
                var riverWaterLevel = await weatherService.GetRiverWatherLevelAsync(CityConstants.UrlRiverWaterLeve);

            // Display results
            DisplayData("Current Weather", currentWeatherData);
            DisplayData("Weather in the Next 3 Days", weatherDataFor3Days);
            DisplayData("River Water Level", riverWaterLevel);
     
           

            static IServiceProvider SetupDependencies()
            {
                return new ServiceCollection()
                    .AddLogging(builder =>
                    {
                        builder.AddConsole(); // Adds the console logger
                    })                   
                    .AddSingleton<HtmlWeb>()
                    .AddScoped<IWeatherService, WeatherService>()
                    .BuildServiceProvider();
            }


            static void DisplayData(string title, List<string> data)
            {

                Console.OutputEncoding = Encoding.UTF8;

                if (data != null)
                {
                    Console.WriteLine($"{title}");
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine(string.Join(Environment.NewLine, data));
                    Console.WriteLine(Environment.NewLine);
                }
            }
        }

    }
}
