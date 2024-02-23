using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatheFishing.Constants
{
    internal class CityConstants
    {
        public static readonly string UrlCurrentWeatherData = "http://www.weather.bg/index.php?koiFail=tekushti&lng=0";
        public static readonly string UrlWeatherDataFor3Day = "http://www.meteo.bg/bg/forecast/gradove";
        public static readonly string UrlRiverWaterLeve = "http://www.meteo.bg/bg/rekiTablitsa";

        public static readonly string StartPointForDatesInWeather = "Град";

        public static readonly string[] CitiesForCurrentWeather = {
        "Монтана",
        "Плевен",
        "Пловдив - Тракия",
        "Пазарджик",
        "Кърджали"
        };

        public static readonly string[] CitiesForFor3DaysWeather =
        {
         "Монтана",
        "Плевен",
        "Пловдив",      
        "Кърджали"
        };

        public static readonly string[] RiversWatherLevelХМС =
        {       
            "71650",
            "71700",
            "71800",
            "72700",
            "72850",
            "73750",
            "73850",
            "16800",
            "16850",
            "18650",
            "18700",
            "18800",
            "74650",           
            "61650",
            "61700",
            "52800",
            "52850",
            "51650",
            "51750",
            "51800 ",
            "51880 ",           
        };
      
    }
}

