using System.Net;
using System.Text.Json;

namespace SolarWatch.Service;

public class CoordAndDateProcessor : ICoordAndDateProcessor
{
    public string GetSunriseTime(float lat, float lon, string date)
    {
        return GetSunRiseSetTime(lat, lon, date).Item1;
    }

    public string GetSunsetTime(float lat, float lon, string date)
    {
        return GetSunRiseSetTime(lat, lon, date).Item2;
    }
    
    private (string, string) GetSunRiseSetTime(float lat, float lon, string date)
    {
        var url = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lon}&date={date}";
        using (var client = new HttpClient())
        {
            var response = client.GetAsync(url).Result;
            var responseJson = response.Content.ReadAsStringAsync().Result;

            if (responseJson is null) 
            {
                return (null, null);
            }
            
            JsonDocument json = JsonDocument.Parse(responseJson);
            JsonElement results = json.RootElement.GetProperty("results");

            string sunrise = results.GetProperty("sunrise").GetString();
            string sunset = results.GetProperty("sunset").GetString();
            
            return (sunrise, sunset);
        }
    }
}