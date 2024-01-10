using SolarWatch.Data;

namespace SolarWatch.Model.Repository;

public class SunTimesRepository : ISunTimesRepository
{
    public IEnumerable<SunTimes> GetAll()
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunTimes.ToList();
    }

    public SunTimes? GetCityName(string cityName)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunTimes.SingleOrDefault(c => c.CityName == cityName);
    }

    public SunTimes? GetByDateAndName(string cityName, DateTime date)
    {
        using var dbContext = new SolarWatchContext();
        return dbContext.SunTimes.SingleOrDefault(c => c.Date == date && c.CityName == cityName);
    }

    public void Add(SunTimes city)
    {
        using var dbContext = new SolarWatchContext();
        dbContext.Add(city);
        dbContext.SaveChanges();
    }

    public void Delete(SunTimes city)
    {
        using var dbContext = new SolarWatchContext();
        dbContext.Remove(city);
        dbContext.SaveChanges();
    }

    public void Update(SunTimes city)
    {
        using var dbContext = new SolarWatchContext();
        dbContext.Update(city);
        dbContext.SaveChanges();
    }
}