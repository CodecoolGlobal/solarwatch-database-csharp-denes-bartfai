namespace SolarWatch.Model;

public class SunTimes
{
    public int Id { get; init; }
    public string? CityName { get; init; }
    public DateTime Sunrise { get; init; }
    public DateTime Sunset { get; init; }
    public DateTime Date { get; init; }
    
}