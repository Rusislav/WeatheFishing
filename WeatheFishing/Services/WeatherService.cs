using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WeatheFishing.Constants;
using WeatheFishing.Contracts;

namespace WeatheFishing.Services
{
    internal class WeatherService : IWeatherService
    {
        public async Task<List<string>> GetCurrentWeatherDataAsync(string urlCurrentWeather)
        {
            var CurrentWeatherData = new List<string>();
            try
            {         

                using (var httpClient = new HttpClient())
                {
                    var html = await httpClient.GetStringAsync(urlCurrentWeather);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    if (CityConstants.CitiesForCurrentWeather != null)
                    {
                        foreach (var city in CityConstants.CitiesForCurrentWeather)
                        {
                            var currentWeatherInCity = htmlDocument.DocumentNode.SelectSingleNode($"//td[text()='{city}']");
                            var dataOfTheWeather = currentWeatherInCity?.SelectNodes("following-sibling::td[position() <= 7]");

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
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return CurrentWeatherData;
        }


        public async Task<List<string>> GetWeatherDataFor3DaysAsync(string urlWeatherFor3Days)
        {
            var WeatherDataFor3Days = new List<string>();

            try
            {
                using (var httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true }))
                {
                    //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                    //var html = httpClient.GetStringAsync(urlWeatherFor3Days).Result;
                    //var doc = new HtmlDocument();
                    //htmlDocument.LoadHtml(html);

                    var web = new HtmlWeb();
                   var doc = await web.LoadFromWebAsync(urlWeatherFor3Days).ConfigureAwait(false);

                    if (CityConstants.CitiesForFor3DaysWeather != null)
                    {
                        foreach (var city in CityConstants.CitiesForFor3DaysWeather)
                        {
                            var WeatherInCityFor3Days = doc.DocumentNode.SelectSingleNode($"//td[text()='{city}']");
                            var dataOfTheWeather = WeatherInCityFor3Days?.SelectNodes("following-sibling::td[position() <= 8]");

                            var DatesWeatherInCityFor3Days = doc.DocumentNode.SelectSingleNode($"//th[text()='Град']");
                            var dataOfTheWeatherDate = DatesWeatherInCityFor3Days?.SelectNodes("following-sibling::th[position() <= 4]");

                            var weatherDataBuilder = new StringBuilder();
                            weatherDataBuilder.AppendLine($"City: {city}");

                            if (dataOfTheWeatherDate != null)
                            {
                                string todayDate = dataOfTheWeatherDate[0]?.InnerText.Trim();
                                string nextDay = dataOfTheWeatherDate[1]?.InnerText.Trim();
                                string inTwoDays = dataOfTheWeatherDate[2]?.InnerText.Trim();
                                string inThreeDays = dataOfTheWeatherDate[3]?.InnerText.Trim();

                                weatherDataBuilder.AppendLine($"Today: {todayDate}");
                                weatherDataBuilder.AppendLine($"Next Day: {nextDay}");
                                weatherDataBuilder.AppendLine($"In Two Days: {inTwoDays}");
                                weatherDataBuilder.AppendLine($"In Three Days: {inThreeDays}");
                            }

                            if (dataOfTheWeather != null)
                            {
                                var todayTemp = dataOfTheWeather[0].SelectNodes(".//span");
                                if (todayTemp != null && todayTemp.Count == 2)
                                {
                                    var lowTemperature = todayTemp[0].InnerHtml.Replace("&nbsp;", "").Trim();
                                    var highTemperature = todayTemp[1].InnerHtml.Replace("&nbsp;", "").Trim();
                                    weatherDataBuilder.AppendLine($"Today Temperature: Low {lowTemperature}High {highTemperature}°C");
                                }

                                string TodayweatherCondition = dataOfTheWeather[1]?.InnerText.Trim();
                                weatherDataBuilder.AppendLine($"Today Weather Condition: {TodayweatherCondition}");

                                var nextDayTemp = dataOfTheWeather[2].SelectNodes(".//span");
                                if (nextDayTemp != null && nextDayTemp.Count == 2)
                                {
                                    var lowTemperature = nextDayTemp[0].InnerHtml.Replace("&nbsp;", "").Trim();
                                    var highTemperature = nextDayTemp[1].InnerHtml.Replace("&nbsp;", "").Trim();
                                    weatherDataBuilder.AppendLine($"Next Day Temperature: Low {lowTemperature}High {highTemperature}°C");
                                }

                                string nextDayweatherCondition = dataOfTheWeather[3]?.InnerText.Trim();
                                weatherDataBuilder.AppendLine($"Next Day Weather Condition: {nextDayweatherCondition}");

                                var inTwoDayTemp = dataOfTheWeather[4].SelectNodes(".//span");
                                if (inTwoDayTemp != null && inTwoDayTemp.Count == 2)
                                {
                                    var lowTemperature = inTwoDayTemp[0].InnerHtml.Replace("&nbsp;", "").Trim();
                                    var highTemperature = inTwoDayTemp[1].InnerHtml.Replace("&nbsp;", "").Trim();
                                    weatherDataBuilder.AppendLine($"In Two Days Temperature: Low {lowTemperature}High {highTemperature}°C");
                                }

                                string inTwoDayweatherCondition = dataOfTheWeather[5]?.InnerText.Trim();
                                weatherDataBuilder.AppendLine($"In Two Days Weather Condition: {inTwoDayweatherCondition}");

                                var inThreeDayTemp = dataOfTheWeather[6].SelectNodes(".//span");
                                if (inThreeDayTemp != null && inThreeDayTemp.Count == 2)
                                {
                                    var lowTemperature = inThreeDayTemp[0].InnerHtml.Replace("&nbsp;", "").Trim();
                                    var highTemperature = inThreeDayTemp[1].InnerHtml.Replace("&nbsp;", "").Trim();
                                    weatherDataBuilder.AppendLine($"In Three Days Temperature: Low {lowTemperature}High {highTemperature}°C");
                                }

                                string inThreeDayweatherCondition = dataOfTheWeather[7]?.InnerText.Trim();
                                weatherDataBuilder.AppendLine($"In Three Days Weather Condition:  {inThreeDayweatherCondition}");
                            }

                            //Console.WriteLine(weatherDataBuilder.ToString());
                            WeatherDataFor3Days.Add(weatherDataBuilder.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return WeatherDataFor3Days;
        }
    }
}
