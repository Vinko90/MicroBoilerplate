using Microsoft.Extensions.Options;
using Npgsql;
using RepoDb;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Repositories.Integration;

namespace Template.Data.Infrastructure;

public class CommonRepository<TEntity> : 
    BaseRepository<TEntity, NpgsqlConnection>,
    IRepository<TEntity, NpgsqlConnection>
    where TEntity : class
{
    private IUnitOfWork<NpgsqlConnection> _unitOfWork;

    public CommonRepository(IOptions<DbSettings> options)
        : base(options.Value.ConnectionString) { }

    public void Attach(IUnitOfWork<NpgsqlConnection> unitOfWork) =>
        _unitOfWork = unitOfWork;

    public TResult Add<TResult>(TEntity entity) =>
        Insert<TResult>(entity,
            transaction: _unitOfWork.Transaction);

    public int AddAll(IEnumerable<TEntity> entities) =>
        InsertAll(entities,
            transaction: _unitOfWork.Transaction);

    public int Delete(object id) =>
        Delete(id,
            transaction: _unitOfWork.Transaction);

    public TResult Merge<TResult>(TEntity entity) =>
        Merge<TResult>(entity,
            transaction: _unitOfWork?.Transaction);

    public TEntity Query(object id) =>
        Query(id,
            transaction: _unitOfWork?.Transaction)?.FirstOrDefault();

    public int Update(TEntity entity) =>
        Update(entity,
            transaction: _unitOfWork?.Transaction);
}
