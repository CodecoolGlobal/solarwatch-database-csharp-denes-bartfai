﻿using System.Net;
using System.Text.Json;

namespace SolarWatch.Service;

public class CoordAndDateProcessor : ICoordAndDateProcessor
{
    public async Task<string> GetSunriseTime(double lat, double lon, string date)
    {
        var (sunrise, _) = await GetSunriseSetTime(lat, lon, date);
        return sunrise;
    }

    public async Task<string> GetSunsetTime(double lat, double lon, string date)
    {
        var (_, sunset) = await GetSunriseSetTime(lat, lon, date);
        return sunset;
    }
    
    private async Task<(string, string)> GetSunriseSetTime(double lat, double lon, string date)
    {
        var url = $"https://api.sunrise-sunset.org/json?lat={lat}&lng={lon}&date={date}";
        using (var client = new HttpClient())
        {
            var responseJson = await client.GetStringAsync(url);

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