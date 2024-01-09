namespace SolarWatch.Model.Repository;

public interface ISunTimesRepository
{
    IEnumerable<SunTimes> GetAll();
    SunTimes? GetCityName(string cityName);
    SunTimes? GetByDateAndName(string cityName, DateTime date);

    void Add(SunTimes sunTimes);
    void Delete(SunTimes sunTimes);
    void Update(SunTimes sunTimes);
}