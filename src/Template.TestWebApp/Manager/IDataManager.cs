using Template.Data.Infrastructure.Entities;

namespace Template.TestWebApp.Manager;

public interface IDataManager
{
    List<WeatherForecastEntity> GetAllItems();

    WeatherForecastEntity GetById(int id);

    int AddNewItem(WeatherForecastEntity item);

    bool RemoveById(int id);
}