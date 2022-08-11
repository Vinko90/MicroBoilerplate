using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Template.Data.Domain.Abstractions;
using Template.Data.Domain.Interfaces;
using Template.Data.Infrastructure.Helpers;

namespace Template.Data.Infrastructure.Abstractions;

internal abstract class Repository<TDomainEntity, TPersistentEntity, TId> : IRepository<TDomainEntity, TId>
    where TDomainEntity : BaseEntity<TId>
    where TPersistentEntity : class
    where TId : IComparable<TId>
{
    protected readonly IDbConnection Connection;
    protected readonly IDbTransaction Transaction;

    protected static readonly string TableName = TableReflectionHelper.GetTableName<TPersistentEntity>();

    protected Repository(IDbConnection connection, IDbTransaction transaction)
    {
        Connection = connection;
        Transaction = transaction;
    }
    
    public async Task<TDomainEntity> GetByIdAsync(TId id, CancellationToken cancellationToken)
    {
        var persistentEntity = await Connection.GetAsync<TPersistentEntity>(id, transaction: Transaction);
        return (persistentEntity == null ? null : MapToDomainEntity(persistentEntity))!;
    }

    public TDomainEntity Add(TDomainEntity entity)
    {
        var persistentEntity = MapToPersistentEntity(entity);
        Connection.Insert(persistentEntity, transaction: Transaction);
        var id = Connection.ExecuteScalar<TId>("lastval()", transaction: Transaction);
        SetPersistentEntityId(persistentEntity, id);
        return MapToDomainEntity(persistentEntity);
    }

    public void Update(TDomainEntity entity)
    {
        var persistentEntity = MapToPersistentEntity(entity);
        Connection.Update(persistentEntity, transaction: Transaction);
    }

    public void Remove(TDomainEntity entity)
    {
        var persistentEntity = MapToPersistentEntity(entity);
        Connection.Delete(persistentEntity, transaction: Transaction);
    }

    protected abstract TPersistentEntity MapToPersistentEntity(TDomainEntity entity);
    
    protected abstract TDomainEntity MapToDomainEntity(TPersistentEntity entity);

    protected abstract void SetPersistentEntityId(TPersistentEntity entity, TId id);
}