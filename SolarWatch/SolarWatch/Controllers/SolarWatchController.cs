using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
using SolarWatch.Service;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class SolarWatchController : ControllerBase
{

    private readonly ILogger<SolarWatchController> _logger;
    private readonly ICityNameProcessor _cityNameProcessor;
    private readonly ICoordAndDateProcessor _coordAndDateProcessor;

    public SolarWatchController(ILogger<SolarWatchController> logger, ICityNameProcessor cityNameProcessor , ICoordAndDateProcessor coordAndDateProcessor)
    {
        _logger = logger;
        _cityNameProcessor = cityNameProcessor;
        _coordAndDateProcessor = coordAndDateProcessor;
    }

    [HttpGet(Name = "GetCurrent")]
    public async Task<ActionResult<SolarWatchProperty>> Get(string cityName, DateTime date)
    {
        string formattedDate = date.ToString("yyyy'-'M'-'d");

        try
        {
            var lat = _cityNameProcessor.GetLatCoord(cityName);
            var lon = _cityNameProcessor.GetLatCoord(cityName);
            _logger.LogInformation($"Data from _cityNameProcessor City:{cityName}---LAT:{lat}, LON:{lon}");

            if (lat == 0)
            {
                _logger.LogError($"Error getting coordinates for city {cityName}");
                return StatusCode(500, $"Error getting coordinates for city {cityName}");
            }

            try
            {
                var sunrise = _coordAndDateProcessor.GetSunriseTime(lat, lon, formattedDate);
                var sunset = _coordAndDateProcessor.GetSunsetTime(lat, lon, formattedDate);
                _logger.LogInformation($"Data from _coordAndDateProcessor --- RISE:{sunrise}, SET:{sunset}");

                return Ok(new SunriseSetCity
                {
                    CityName = cityName,
                    Date = date,
                    Sunrise = sunrise,
                    Sunset = sunset
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting sunrise and sunset times for city {cityName} at date {formattedDate}");
                return StatusCode(500,
                    $"Error getting sunrise and sunset times for city {cityName} at date {formattedDate}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error getting coordinates for city {cityName}");
            return StatusCode(500, $"Error getting coordinates for city {cityName}");
        }
    }
}