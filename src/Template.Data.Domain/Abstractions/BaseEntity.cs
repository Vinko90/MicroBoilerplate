namespace Template.Data.Domain.Abstractions;

public abstract class BaseEntity<TId> where TId : IComparable<TId>
{
    public TId Id { get; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(TId id)
    {
        Id = id;
    }
}