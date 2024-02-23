using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Xml.XPath;
using WeatheFishing.Constants;
using WeatheFishing.Contracts;
using WeatheFishing.Model;

namespace WeatheFishing.Services
{
    internal class WeatherService : IWeatherService
    {

        private readonly ILogger<WeatherService> _logger;
        private readonly HtmlWeb _htmlWeb;

        /// <summary>
        /// Injecting the constructor.
        /// </summary>
        /// <param name="htmlWeb"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public WeatherService(HtmlWeb htmlWeb, ILogger<WeatherService> logger)
        {
            _htmlWeb = htmlWeb ?? throw new ArgumentNullException(nameof(htmlWeb));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// List where I hold the proper rivers for fishing.
        /// </summary>
        List<RiverData> bestRiversForFishing = new List<RiverData>();


        /// <summary>
        /// Get the data for the present weather from the web server using HtmlAgilityPack and save the data in a List.
        /// </summary>
        /// <param name="urlCurrentWeather"></param>
        /// <returns>List with the taken data.</returns>
        public async Task<List<string>> GetCurrentWeatherDataAsync(string urlCurrentWeather)
        {
            var currentWeatherData = new List<string>();
            try
            {            
                var htmlDocument = await LoadHtmlDocumentAsync(urlCurrentWeather);             

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
                        else
                        {
                            throw new ArgumentNullException("Null  argument is passed to a method or constructor");
                        }
                    }
                }

                _logger.LogInformation("GetCurrentWeatherDataAsync executed successfully");

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error in GetCurrentWeatherDataAsync");
            }
            catch (HtmlWebException ex)
            {
                _logger.LogError(ex, "Error during web requests check urls, network problems or timeouts");
            }
            catch (XPathException ex)
            {
                _logger.LogError(ex, "Error in XPath expression. Check the XPath query for correctness.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during weather data retrieval.");
            }

            return currentWeatherData;
        }


        /// <summary>
        /// Get the weather data for the next three days from the web server using HtmlAgilityPack and save the data in a List.
        /// </summary>
        /// <param name="urlWeatherFor3Days"></param>
        /// <returns>List with the taken data</returns>
        public async Task<List<string>> GetWeatherDataFor3DaysAsync(string urlWeatherFor3Days)
        {
            var weatherDataFor3Days = new List<string>();

            try
            {
                var doc = await _htmlWeb.LoadFromWebAsync(urlWeatherFor3Days);

                if (CityConstants.CitiesForFor3DaysWeather != null)
                {
                    foreach (var city in CityConstants.CitiesForFor3DaysWeather)
                    {
                        var weatherInCityFor3Days = doc.DocumentNode.SelectSingleNode($"//td[text()='{city}']");
                        var dataOfTheWeather = weatherInCityFor3Days?.SelectNodes("following-sibling::td[position() <= 8]");

                        var datesWeatherInCityFor3Days = doc.DocumentNode.SelectSingleNode($"//th[text()='{CityConstants.StartPointForDatesInWeather}']");
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
                                AppendTemperatureInfo(weatherDataBuilder, dataOfTheWeather);

                                for (int i = 0; i < Math.Min(dataOfTheWeather.Count, 4); i++)
                                {
                                    string weatherCondition = dataOfTheWeather[i + 1]?.InnerText.Trim();
                                weatherDataBuilder.AppendLine($"{GetDayLabel(i)} Weather Condition: {weatherCondition}");
                                }                                                  
                        }
                        else
                        {
                            throw new XPathException("No node match the Xpath expression!");
                        }

                        weatherDataFor3Days.Add(weatherDataBuilder.ToString());
                    }
                }
                _logger.LogInformation("GetWeatherDataFor3DaysAsync executed successfully");
            }

            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "A null argument is passed to a method or constructor");
            }
            catch (HtmlWebException ex)
            {
                _logger.LogError(ex, "Error during web request. Check network connectivity and URL validity.");
            }
            catch (XPathException ex)
            {
                _logger.LogError(ex, "Error in XPath expression. Check the XPath query for correctness.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during weather data retrieval.");
            }

            return weatherDataFor3Days;
        }

        /// <summary>
        /// Get the river level water data from the web server using HtmlAgilityPack and save the data in a List then use
        /// the methods BestRiverToFish and IsRiverSuitableForFishing to add the rivers to the List and judge whether a given river is fishable.
        /// </summary>
        /// <param name="urlRiverWaterLevel"></param>
        /// <returns>List with the taken data and conclusion whether a given river is suitable for fishing. </returns>
        public async Task<List<string>> GetRiverWatherLevelAsync(string urlRiverWaterLevel)
        {
            var riverWatherLevelData = new List<string>();
            var bestRiversBuilder = new StringBuilder();
            try
            {
                var doc = await _htmlWeb.LoadFromWebAsync(urlRiverWaterLevel);

                var tdElements = doc.DocumentNode.SelectNodes("//td");



                if (CityConstants.RiversWatherLevelХМС != null)
                {
                    foreach (var xmc in CityConstants.RiversWatherLevelХМС) {


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

                            BestRiverToFish(river, hydrometricStation, double.Parse(minimumFloatRaterOfTheRiver), double.Parse(nominalFloatRaterOfTheRiver),
                                  double.Parse(maximumFloatRaterOfTheRiver), int.Parse(depthOfTheWater), double.Parse(flowRate), int.Parse(changeInHeightOfTheRiver));

                            riverLevelDataBuilder.AppendLine($"River: {river}");
                            riverLevelDataBuilder.AppendLine($"Location of hydrometric station: {hydrometricStation} \n" +
                            $"Minimum flow rate of the river is {minimumFloatRaterOfTheRiver} [m³/s] \n" +
                            $"Nominal flow rate of the river is {nominalFloatRaterOfTheRiver} [m³/s] \n" +
                            $"Maximum flow rate of the river is {maximumFloatRaterOfTheRiver} [m³/s] \n" +
                            $"The depth of the water is {depthOfTheWater} [cm] \n" +
                            $"The current flow rate of the river {flowRate} [m3/s] \n" +
                            $"The change in height of the river is {changeInHeightOfTheRiver} [cm]");

                        }
                        else
                        {
                            throw new XPathException("No node match the Xpath expression!");
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
                _logger.LogInformation("GetRiverWatherLevelAsync executed successfully");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "A null argument is passed to a method or constructor");
            }
            catch (HtmlWebException ex)
            {
                _logger.LogError(ex, "Error during web request. Check network connectivity and URL validity.");
            }
            catch (XPathException ex)
            {
                _logger.LogError(ex, "Error in XPath expression. Check the XPath query for correctness.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during weather data retrieval.");
            }


            bestRiversBuilder.AppendLine("Best rivers to fish today are:");
            foreach (var river in bestRiversForFishing)
            {
                bestRiversBuilder.AppendLine($"Fishing conditions are good on this river {river.River} in the region of {river.HydrometricStation} " +
                    $"the depth of the water is {river.DepthOfTheWater} [cm] and the level of the water is changed by {river.ChangeInHeightOfTheRiver} [cm]");
            }
            riverWatherLevelData.Add(bestRiversBuilder.ToString());


            return riverWatherLevelData;
        }


        /// <summary>
        /// Adds the rivers that have been checked by the IsRiverSuitableForFishing method to the List.
        /// </summary>
        /// <param name="river"></param>
        /// <param name="hydrometricStation"></param>
        /// <param name="minimumFloatRate"></param>
        /// <param name="nominalFloatRate"></param>
        /// <param name="maximumFloatRate"></param>
        /// <param name="depthOfTheWater"></param>
        /// <param name="currentFlowRate"></param>
        /// <param name="changeInHeightOfTheRiver"></param>
        /// <returns>List with the rivers.</returns>
        public List<RiverData> BestRiverToFish(string river, string hydrometricStation, double minimumFloatRate, double nominalFloatRate, double maximumFloatRate, int depthOfTheWater, double currentFlowRate, int changeInHeightOfTheRiver)
        {
            try
            {
                if (IsRiverSuitableForFishing(nominalFloatRate, currentFlowRate, changeInHeightOfTheRiver))
                {
                    RiverData bestRiver = new RiverData()
                    {
                        River = river,
                        HydrometricStation = hydrometricStation,
                        MinimumFloatRateOfTheRiver = minimumFloatRate,
                        NominalFloatRateOfTheRiver = nominalFloatRate,
                        MaximumFloatRateOfTheRiver = maximumFloatRate,
                        DepthOfTheWater = depthOfTheWater,
                        FlowRate = currentFlowRate,
                        ChangeInHeightOfTheRiver = changeInHeightOfTheRiver
                    };

                    bestRiversForFishing.Add(bestRiver);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during the select of river.");
            }

            return bestRiversForFishing;
        }


        /// <summary>
        /// Checks whether a given river is suitable for fishing.
        /// </summary>
        /// <param name="nominalFloatRate"></param>
        /// <param name="currentFlowRate"></param>
        /// <param name="changeInHeightOfTheRiver"></param>
        /// <returns>True or false</returns>
        private bool IsRiverSuitableForFishing(double nominalFloatRate, double currentFlowRate, int changeInHeightOfTheRiver)
        {
            if (changeInHeightOfTheRiver > 0 && changeInHeightOfTheRiver <= 3)
            {
                return (nominalFloatRate > currentFlowRate) && (nominalFloatRate - currentFlowRate < 3000);
            }
            else if (changeInHeightOfTheRiver < 0 && changeInHeightOfTheRiver >= -3)
            {
                return (nominalFloatRate > currentFlowRate) && (nominalFloatRate - currentFlowRate < 3000);
            }

            return false;
        }

        /// <summary>
        /// Asynchronously load HTML content from a specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>HTML content</returns>
        private async Task<HtmlDocument> LoadHtmlDocumentAsync(string url)
        {
            try
            {
                var html = await _htmlWeb.LoadFromWebAsync(url);
                return html;
            }
            catch (HtmlWebException ex)
            {
                _logger.LogError(ex, "Error during web request. Check network connectivity and URL validity.");
                throw; // Rethrow the exception for higher-level error handling.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during HTML document loading.");
                throw; // Rethrow the exception for higher-level error handling.
            }
        }
        /// <summary>
        /// Gets the temperature data for the given day.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="data"></param>
        private void AppendTemperatureInfo(StringBuilder builder, HtmlNodeCollection data)
        {
            for (int i = 0; i < Math.Min(data.Count, 4); i++)
            {
                var tempNode = data[i].SelectNodes(".//span");

                if (tempNode != null && tempNode.Count == 2)
                {
                    var lowTemperature = tempNode[0].InnerHtml.Replace("&nbsp;", "").Trim();
                    var highTemperature = tempNode[1].InnerHtml.Replace("&nbsp;", "").Trim();
                    builder.AppendLine($"{GetDayLabel(i)} Temperature: Low {lowTemperature}High {highTemperature}°C");
                }
            }
        }
        /// <summary>
        /// Returns a day that specifies a loop iteration.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Returns a specific day</returns>
        private string GetDayLabel(int index)
        {
            switch (index)
            {
                case 0: return "Today";
                case 1: return "Next Day";
                case 2: return "In Two Days";
                case 3: return "In Three Days";
                default: return $"Day {index + 1}";
            }
        }
    }
}
