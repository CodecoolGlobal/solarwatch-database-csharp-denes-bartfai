using System.Net;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SolarWatch.Service;

public class CityNameProcessor : ICityNameProcessor
{
    
    public async Task<double> GetLatCoord(string cityName)
    {
        var (lat, _, _, _) = await GetInfo(cityName);
        return lat;
    }
    public async Task<double> GetLonCoord(string cityName)
    {
        var (_, lon, _, _) = await GetInfo(cityName);
        return lon;
    }

    public async Task<string> GetState(string cityName)
    {
        var (_, _, state, _) = await GetInfo(cityName);
        return state;
    }
    
    public async Task<string> GetCountry(string cityName)
    {
        var (_, _, _, country) = await GetInfo(cityName);
        return country;
    }
    

    private async Task<(double, double, string?, string?)> GetInfo(string cityName)
    {
        var apiKey = "c047c04a6fd12ce4ec1c75b11ffe85ec";
        var url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";
        using(var client = new HttpClient())
        {
            var responseJson = await client.GetStringAsync(url);

            var locations = JsonConvert.DeserializeObject<Location[]>(responseJson);

            if (locations.Length > 0)
            {
                var lat = locations[0].lat;
                var lon = locations[0].lon;
                var state = locations[0].state;
                var country = locations[0].country;

                return (lat, lon, state, country);
            }
            else
            {
                return (0, 0, null, null);
            }
        }
    }
}

public class Location
{
    public double lat { get; set; }
    public double lon { get; set; }
    public string? state { get; set; }
    public string country { get; set; }
}