using System.Net;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SolarWatch.Service;

public class CityNameProcessor : ICityNameProcessor
{
    public float GetLonCoord(string cityName)
    {
        return GetCoords(cityName).Item1;
    }

    public float GetLatCoord(string cityName)
    {
        return GetCoords(cityName).Item2;
    }

    private (float, float) GetCoords(string cityName)
    {
        var apiKey = "c047c04a6fd12ce4ec1c75b11ffe85ec";
        var url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";
        using(var client = new WebClient())
        {
            var responseJson = client.DownloadString(url);

            var locations = JsonConvert.DeserializeObject<Location[]>(responseJson);

            if (locations.Length > 0)
            {
                var lat = locations[0].lat;
                var lon = locations[0].lon;

                return (lat, lon);
            }
            else
            {
                return (0, 0);
            }
        }
    }
}

public class Location
{
    public float lat { get; set; }
    public float lon { get; set; }
}