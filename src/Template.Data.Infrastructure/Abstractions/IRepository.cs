using System.Data;

namespace Template.Data.Infrastructure.Abstractions;

public interface IRepository<TEntity, TDbConnection>
    where TEntity : class
    where TDbConnection : IDbConnection
{
    void Attach(IUnitOfWork<TDbConnection> unitOfWork);
    TResult Add<TResult>(TEntity entity);
    int AddAll(IEnumerable<TEntity> entities);
    int Delete(object id);
    TResult Merge<TResult>(TEntity entity);
    TEntity Query(object id);
    int Update(TEntity entity);
}