using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatheFishing.Contracts
{
    internal interface IWeatherService
    {
        Task<List<string>> GetCurrentWeatherDataAsync(string urlCurrentWeather);
       Task<List<string>> GetWeatherDataFor3DaysAsync(string urlWeatherFor3Days);
    }
}
