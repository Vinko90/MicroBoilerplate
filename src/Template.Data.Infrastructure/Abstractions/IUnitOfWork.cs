using System.Data;
using System.Data.Common;

namespace Template.Data.Infrastructure.Abstractions;

public interface IUnitOfWork<TDbConnection> 
    where TDbConnection : IDbConnection
{
    TDbConnection Connection { get; }
    DbTransaction Transaction { get; }
    void Begin();
    void Rollback();
    void Commit();
}