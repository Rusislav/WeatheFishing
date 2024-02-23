using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatheFishing.Model;

namespace WeatheFishing.Contracts
{
    internal interface IWeatherService
    {
        Task<List<string>> GetCurrentWeatherDataAsync(string urlCurrentWeather);
       Task<List<string>> GetWeatherDataFor3DaysAsync(string urlWeatherFor3Days);

        Task<List<string>> GetRiverWatherLevelAsync(string urlRiverWaterLevel);

        List<RiverData> BestRiverToFish(string river,string hydrometricStation, double minimumFloatRaterOfTheRiver,
            double nominalFloatRaterOfTheRiver, double maximumFloatRaterOfTheRiver ,int depthOfTheWater, double currentflowRate, int changeInHeightOfTheRiver);

       
    }
}
