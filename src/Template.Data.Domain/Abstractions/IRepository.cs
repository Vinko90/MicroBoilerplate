using Template.Data.Domain.Abstractions;

namespace Template.Data.Domain.Interfaces;

public interface IRepository<TEntity, in TId> where TEntity : BaseEntity<TId> where TId : IComparable<TId>
{
    Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken);
        
    TEntity Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);
}