using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SolarWatch.Controllers;
using SolarWatch.Service;

namespace SolarWatchTest;

public class SolarWatchControllerTest
{
    private Mock<ILogger<SolarWatchController>> _loggerMock;
    private Mock<ICityNameProcessor> _cityNameProcessor;
    private Mock<ICoordAndDateProcessor> _coordAndDateProcessor;
    private SolarWatchController _controller;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<SolarWatchController>>();
        _cityNameProcessor = new Mock<ICityNameProcessor>();
        _coordAndDateProcessor = new Mock<ICoordAndDateProcessor>();
        _controller = new SolarWatchController(_loggerMock.Object, _cityNameProcessor.Object, _coordAndDateProcessor.Object);
    }

    [Test]
    public async Task Get_ReturnsOkResult_WithValidData()
    {
        //Arrange
        var cityName = "Budapest";
        var date = DateTime.Parse("2023-02-03");
        
        float lat = 47.497993f;
        float lon = 19.04036f;

        var sunrise = "5:16:19 AM";
        var sunset = "4:39:11 PM";
        
        _cityNameProcessor.Setup(x => x.GetLatCoord(cityName)).Returns(lat);
        _cityNameProcessor.Setup(x => x.GetLonCoord(cityName)).Returns(lon);

        _coordAndDateProcessor.Setup(x => x.GetSunriseTime(lat, lon, date.ToString("yyyy'-'M'-'d"))).Returns(sunrise);
        _coordAndDateProcessor.Setup(x => x.GetSunsetTime(lat, lon, date.ToString("yyyy'-'M'-'d"))).Returns(sunset);

        // Act
        var result = await _controller.Get(cityName, date);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<OkObjectResult>(result.Result);
    }
}