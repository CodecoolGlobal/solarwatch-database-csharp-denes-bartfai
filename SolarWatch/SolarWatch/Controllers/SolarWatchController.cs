using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
using SolarWatch.Model.Repository;
using SolarWatch.Service;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class SolarWatchController : ControllerBase
{

    private readonly ILogger<SolarWatchController> _logger;
    private readonly ICityNameProcessor _cityNameProcessor;
    private readonly ICoordAndDateProcessor _coordAndDateProcessor;
    private readonly ICityRepository _cityRepository;
    private readonly ISunTimesRepository _sunTimesRepository;

    public SolarWatchController(
        ILogger<SolarWatchController> logger,
        ICityNameProcessor cityNameProcessor,
        ICoordAndDateProcessor coordAndDateProcessor,
        ICityRepository cityRepository,
        ISunTimesRepository sunTimesRepository
        )
    {
        _logger = logger;
        _cityNameProcessor = cityNameProcessor;
        _coordAndDateProcessor = coordAndDateProcessor;
        _cityRepository = cityRepository;
        _sunTimesRepository = sunTimesRepository;
    }

    [HttpGet(Name = "GetCurrent")]
    public async Task<ActionResult<SunriseSetCity>> Get(string cityName, DateTime date)
    {
        string formattedDate = date.ToString("yyyy'-'M'-'d");
        double lat = 0;
        double lon = 0;

        var city = _cityRepository.GetByName(cityName);

        if (city == null)
        {
            _logger.LogInformation("City is not in the database, looking further info in API");

            try
            {
                lat = await _cityNameProcessor.GetLatCoord(cityName);
                lon = await _cityNameProcessor.GetLatCoord(cityName);
                _logger.LogInformation($"Data from _cityNameProcessor City:{cityName}---LAT:{lat}, LON:{lon}");

                if (lat == 0)
                {
                    _logger.LogError($"Error getting coordinates for city {cityName}");
                    return StatusCode(500, $"Error getting coordinates for city {cityName}");
                }

                var state = await _cityNameProcessor.GetState(cityName);
                var country = await _cityNameProcessor.GetCountry(cityName);

                _logger.LogInformation
                    ($"Data from _cityNameProcessor City: {cityName} --- LAT:{lat}, LON:{lon}, STATE:{state}, COUNTRY:{country}");

                _cityRepository.Add(new City()
                {
                    Name = cityName,
                    Country = country,
                    State = state,
                    Latitude = lat,
                    Longitude = lon,
                });
                _logger.LogInformation(
                    $"New info added to the database --- CITY: {cityName} --- LAT:{lat}, LON:{lon}, STATE:{state}, COUNTRY:{country}");

                try
                {
                    var (sunrise, sunset) = await GetRiseAndSetWithDateType(lat, lon, formattedDate);

                    _sunTimesRepository.Add(new SunTimes()
                    {
                        CityName = cityName,
                        Date = date,
                        Sunrise = sunrise,
                        Sunset = sunset
                    });
                    _logger.LogInformation(
                        $"New info added to the database --- CITY: {cityName} --- DATE:{date}, SUNRISE:{sunrise}, SUNSET:{sunset}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"Error getting coordinates for city {cityName}");
                    return StatusCode(500,
                        $"Error getting coordinates for city {cityName}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting coordinates for city {cityName}");
                return StatusCode(500, $"Error getting coordinates for city {cityName}");
            }
        }

        var sunTimesForDate = _sunTimesRepository.GetByDateAndName(cityName, date);
        if (sunTimesForDate == null)
        {
            var (sunrise, sunset) = await GetRiseAndSetWithDateType(lat, lon, formattedDate);

            _sunTimesRepository.Add(new SunTimes()
            {
                CityName = cityName,
                Date = date,
                Sunrise = sunrise,
                Sunset = sunset
            });
            _logger.LogInformation(
                $"New info added to the database --- CITY: {cityName} --- DATE:{date}, SUNRISE:{sunrise}, SUNSET:{sunset}");
        }

        return Ok(new SunriseSetCity
        {
            CityName = cityName,
            Date = date,
            Sunrise = _sunTimesRepository.GetByDateAndName(cityName, date).Sunrise,
            Sunset = _sunTimesRepository.GetByDateAndName(cityName, date).Sunset
        });
    }
    
    private async Task<(DateTime, DateTime)> GetRiseAndSetWithDateType(double lat, double lon, string formattedDate)
    {
        var sunriseTime = await _coordAndDateProcessor.GetSunriseTime(lat, lon, formattedDate);
        var sunsetTime = await _coordAndDateProcessor.GetSunsetTime(lat, lon, formattedDate);
        
        _logger.LogInformation(
            $"Data from _coordAndDateProcessor ---- SUNRISE:{sunriseTime}, SET: {sunsetTime}");

        string timeFormat = "h:mm:ss tt";
        DateTime sunrise = DateTime.ParseExact(sunriseTime, timeFormat, CultureInfo.InvariantCulture);
        DateTime sunset = DateTime.ParseExact(sunsetTime, timeFormat, CultureInfo.InvariantCulture);
        Console.WriteLine($"From GetRiseAndSet: set:{sunset}");

        return (sunrise, sunset);
    }
}