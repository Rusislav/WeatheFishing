using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using WeatheFishing.Constants;
using WeatheFishing.Contracts;
using WeatheFishing.Model;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatheFishing.Services
{
    internal class WeatherService : IWeatherService
    {
        private static readonly HttpClient _httpClient = new HttpClient();


        List<RiverData> bestRiversForFishing = new List<RiverData>();
        public async Task<List<string>> GetCurrentWeatherDataAsync(string urlCurrentWeather)
        {
            var currentWeatherData = new List<string>();
            try
            {         

               
                    var html = await _httpClient.GetStringAsync(urlCurrentWeather);
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
                                currentWeatherData.Add(formattedData);
                            }
                        }
                    }
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return currentWeatherData;
        }


        public async Task<List<string>> GetWeatherDataFor3DaysAsync(string urlWeatherFor3Days)
        {
            var weatherDataFor3Days = new List<string>();

            try
            {
                               
                    //var html = httpClient.GetStringAsync(urlWeatherFor3Days).Result;
                    //var doc = new HtmlDocument();
                    //htmlDocument.LoadHtml(html);

                    var web = new HtmlWeb();
                   var doc = await web.LoadFromWebAsync(urlWeatherFor3Days).ConfigureAwait(false);

                    if (CityConstants.CitiesForFor3DaysWeather != null)
                    {
                        foreach (var city in CityConstants.CitiesForFor3DaysWeather)
                        {
                            var weatherInCityFor3Days = doc.DocumentNode.SelectSingleNode($"//td[text()='{city}']");
                            var dataOfTheWeather = weatherInCityFor3Days?.SelectNodes("following-sibling::td[position() <= 8]");

                            var datesWeatherInCityFor3Days = doc.DocumentNode.SelectSingleNode($"//th[text()='Град']");
                            var dataOfTheWeatherDate = datesWeatherInCityFor3Days?.SelectNodes("following-sibling::th[position() <= 4]");

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

                                string todayweatherCondition = dataOfTheWeather[1]?.InnerText.Trim();
                                weatherDataBuilder.AppendLine($"Today Weather Condition: {todayweatherCondition}");

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
                            weatherDataFor3Days.Add(weatherDataBuilder.ToString());
                        }
                    }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return weatherDataFor3Days;
        }

        public async Task<List<string>> GetRiverWatherLevelAsync(string urlRiverWaterLevel)
        {
            var riverWatherLevelData = new List<string>();
            var bestRiversBuilder = new StringBuilder();
            try
            {
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(urlRiverWaterLevel).ConfigureAwait(false);

                var tdElements = doc.DocumentNode.SelectNodes("//td");

              

                if (CityConstants.RiversWatherLevelХМС != null)
                {
                    foreach (var xmc in CityConstants.RiversWatherLevelХМС){

                      
                        var riverLevel = doc.DocumentNode.SelectSingleNode($"//tr/td[contains(., '{xmc}')]");
                        var dataOfTheRiverLevel = riverLevel?.SelectNodes("following-sibling::td[position() <= 8]");

                        var riverLevelDataBuilder = new StringBuilder();

                        if (dataOfTheRiverLevel != null)
                        {
                            string river = dataOfTheRiverLevel[0]?.InnerText.Trim();
                            string hydrometricStation = dataOfTheRiverLevel[1]?.InnerText.Trim();
                            string minimumFloatRaterOfTheRiver = dataOfTheRiverLevel[2]?.InnerText.Trim().Replace(" ", "");
                            string nominalFloatRaterOfTheRiver = dataOfTheRiverLevel[3]?.InnerText.Trim().Replace(" ", "");
                            string maximumFloatRaterOfTheRiver = dataOfTheRiverLevel[4]?.InnerText.Trim().Replace(" ", "");
                            string depthOfTheWater = dataOfTheRiverLevel[5]?.InnerText.Trim();
                            string flowRate = dataOfTheRiverLevel[6]?.InnerText.Trim().Replace(" ", "");
                            string changeInHeightOfTheRiver = dataOfTheRiverLevel[7]?.InnerText.Trim();

                          BestRiverToFish(river, hydrometricStation,double.Parse(minimumFloatRaterOfTheRiver), double.Parse(nominalFloatRaterOfTheRiver),
                                double.Parse(maximumFloatRaterOfTheRiver), int.Parse(depthOfTheWater), double.Parse(flowRate), int.Parse(changeInHeightOfTheRiver));

                            riverLevelDataBuilder.AppendLine($"River: {river}");
                            riverLevelDataBuilder.AppendLine($"Location of hydrometric station: {hydrometricStation} \n " +
                            $"Minimum flow rate of the river is {minimumFloatRaterOfTheRiver} [m³/s] \n" +
                            $"Nominal flow rate of the river is {nominalFloatRaterOfTheRiver} [m³/s] \n" +
                            $"Maximum flow rate of the river is {maximumFloatRaterOfTheRiver} [m³/s] \n" +
                            $"The depth of the water is {depthOfTheWater} [cm] \n" +
                            $"The current flow rate of the river {flowRate} [m3/s] \n" +
                            $"The change in height of the river is {changeInHeightOfTheRiver} [cm]");

                        }
                       
                        riverWatherLevelData.Add(riverLevelDataBuilder.ToString());
                    }


                    //Water meters are primarily classified by size. That is, the nominal diameter of the water meter is matched to the
                    //size of the pipe to which it is being connected. However, it should always be confirmed that the expected flow
                    //rates through the meter fall within the accurate flow range of the meter. The meters flow range is defined as
                    //follows:
                    //Nominal flow rate Qn The designation flow rate of the meter.
                    //Maximum flow rate Qmax The highest flow rate at which the meter accuracy will be within the maximum
                    //permitted error.
                    //Minimum flow rate Qmin The lowest flow at which the meter accuracy will be within the maximum
                    //permitted error.
                    //The symbol "ΔH" typically represents a change in height or elevation, and when accompanied by the unit "[cm]," it likely refers to a change in
                    //    height measured in centimeters.This can be relevant in various contexts, including river stations and hydrology.

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            foreach (var river in bestRiversForFishing)
            {
                int count = 0;
                count++;
                bestRiversBuilder.AppendLine($"Fishing conditions are good on this river {river.River} in the region of {river.HydrometricStation} " +
                    $"the depth of the water is {river.DepthOfTheWater} [cm] and the level of the water is changed by {river.ChangeInHeightOfTheRiver} [cm]");
            }
            riverWatherLevelData.Add(bestRiversBuilder.ToString());


            return riverWatherLevelData;
        }
    

        public List<RiverData> BestRiverToFish(string river, string hydrometricStation, double minimumFloatRaterOfTheRiver, double nominalFloatRaterOfTheRiver, double maximumFloatRaterOfTheRiver, int depthOfTheWater, double currentflowRate, int changeInHeightOfTheRiver)
        {
          

            if (changeInHeightOfTheRiver > 0 && changeInHeightOfTheRiver <= 3)
            {
                if (nominalFloatRaterOfTheRiver > currentflowRate)
                {
                    if (nominalFloatRaterOfTheRiver - currentflowRate < 3000)
                    {
                        RiverData bestRiver = new RiverData()
                        {
                            River = river,
                            HydrometricStation = hydrometricStation,
                            MinimumFloatRateOfTheRiver =  minimumFloatRaterOfTheRiver,
                            NominalFloatRateOfTheRiver = nominalFloatRaterOfTheRiver,
                            MaximumFloatRateOfTheRiver = maximumFloatRaterOfTheRiver,
                            DepthOfTheWater = depthOfTheWater,
                            FlowRate = currentflowRate,
                            ChangeInHeightOfTheRiver = changeInHeightOfTheRiver
                        };
                        bestRiversForFishing.Add(bestRiver);

                    }
                   
                }
                else if (currentflowRate - nominalFloatRaterOfTheRiver < 3000)
                {
                    RiverData bestRiver = new RiverData()
                    {
                        River = river,
                        HydrometricStation = hydrometricStation,
                        MinimumFloatRateOfTheRiver = minimumFloatRaterOfTheRiver,
                        NominalFloatRateOfTheRiver = nominalFloatRaterOfTheRiver,
                        MaximumFloatRateOfTheRiver = maximumFloatRaterOfTheRiver,
                        DepthOfTheWater = depthOfTheWater,
                        FlowRate = currentflowRate,
                        ChangeInHeightOfTheRiver = changeInHeightOfTheRiver
                    };
                    bestRiversForFishing.Add(bestRiver);
                }
            }
            else if (changeInHeightOfTheRiver < 0 && changeInHeightOfTheRiver >= -3)
            {
                if (nominalFloatRaterOfTheRiver > currentflowRate)
                {
                    if (nominalFloatRaterOfTheRiver - currentflowRate < 3000)
                    {
                        RiverData bestRiver = new RiverData()
                        {
                            River = river,
                            HydrometricStation = hydrometricStation,
                            MinimumFloatRateOfTheRiver = minimumFloatRaterOfTheRiver,
                            NominalFloatRateOfTheRiver = nominalFloatRaterOfTheRiver,
                            MaximumFloatRateOfTheRiver = maximumFloatRaterOfTheRiver,
                            DepthOfTheWater = depthOfTheWater,
                            FlowRate = currentflowRate,
                            ChangeInHeightOfTheRiver = changeInHeightOfTheRiver
                        };
                        bestRiversForFishing.Add(bestRiver);

                    }
                }
                else if (currentflowRate - nominalFloatRaterOfTheRiver < 3000)
                {
                    RiverData bestRiver = new RiverData()
                    {
                        River = river,
                        HydrometricStation = hydrometricStation,
                        MinimumFloatRateOfTheRiver = minimumFloatRaterOfTheRiver,
                        NominalFloatRateOfTheRiver = nominalFloatRaterOfTheRiver,
                        MaximumFloatRateOfTheRiver = maximumFloatRaterOfTheRiver,
                        DepthOfTheWater = depthOfTheWater,
                        FlowRate = currentflowRate,
                        ChangeInHeightOfTheRiver = changeInHeightOfTheRiver
                    };
                    bestRiversForFishing.Add(bestRiver);
                }
            }
            
            return bestRiversForFishing;
        }
    }
}
