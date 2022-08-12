using Npgsql;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.TestWebApp.Manager;

public class DataManager : IDataManager
{
    private readonly IUnitOfWork<NpgsqlConnection> _unitOfWork;
    private readonly IWeatherForecastRepository _repo;

    public DataManager(IUnitOfWork<NpgsqlConnection> unitOfWork,
        IWeatherForecastRepository repo)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        
        _repo.Attach(_unitOfWork);
    }
    
    public List<WeatherForecastEntity> GetAllItems()
    {
        throw new NotImplementedException();
    }

    public WeatherForecastEntity GetById(int id)
    {
        _unitOfWork.Begin();
        
        try
        {
            var items = _repo.Query(id);
            _unitOfWork.Commit();
            return items;
        }
        catch (Exception e)
        {
            
            _unitOfWork.Rollback();
            throw;
        }
    }

    public int AddNewItem(WeatherForecastEntity item)
    {
        throw new NotImplementedException();
    }

    public bool RemoveById(int id)
    {
        throw new NotImplementedException();
    }
}