using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatheFishing.Contracts
{
    internal interface IWeatherService
    {
        List<string> GetCurrentWeatherData(string urlCurrentWeather);
    }
}
