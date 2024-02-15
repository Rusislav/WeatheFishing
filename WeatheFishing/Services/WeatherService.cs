using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatheFishing.Constants;
using WeatheFishing.Contracts;

namespace WeatheFishing.Services
{
    internal class WeatherService : IWeatherService
    {
        public List<string> GetCurrentWeatherData(string urlCurrentWeather)
        {
            // The list i will have the information for the weather
            var CurrentWeatherData = new List<string>();

            //Send get requst to weather.bg
            var httpClient = new HttpClient();
            var html = httpClient.GetStringAsync(urlCurrentWeather).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            if (CityConstants.Cities != null)
            {
                foreach (var city in CityConstants.Cities)
                {
                    var currentWeatherInCity = htmlDocument.DocumentNode.SelectSingleNode($"//td[text()='{city}']");
                    var dataOfTheWeather = currentWeatherInCity.SelectNodes("following-sibling::td[position() <= 7]");

                    if (dataOfTheWeather != null)
                    {
                        string date = dataOfTheWeather[0]?.InnerText.Trim();
                        string time = dataOfTheWeather[1]?.InnerText.Trim();
                        string temperature = dataOfTheWeather[2]?.InnerText.Trim();
                        string weatherCondition = dataOfTheWeather[3]?.InnerText.Trim();
                        string windSpeed = dataOfTheWeather[4]?.InnerText.Trim();
                        string windDirection = dataOfTheWeather[5]?.InnerText.Trim();
                        string pressure = dataOfTheWeather[6]?.InnerText.Trim();

                        string formattedData = $"{city} - Date: {date}, Time: {time} часа, Temperature: {temperature} °C, Weather: {weatherCondition}, Wind Speed: {windSpeed} m/s, Wind Direction: {windDirection}, Pressure: {pressure} hPa";
                        CurrentWeatherData.Add(formattedData);
                    }
                }

            }
            return CurrentWeatherData;
        }
    }
}
