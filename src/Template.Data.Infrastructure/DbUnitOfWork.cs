using System.Data.Common;
using Microsoft.Extensions.Options;
using Npgsql;
using RepoDb;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Repositories.Integration;

namespace Template.Data.Infrastructure;

public class DbUnitOfWork : IUnitOfWork<NpgsqlConnection>
{
    private DbSettings _dbSettings;
    private NpgsqlConnection _connection;
    private DbTransaction _transaction;

    public DbUnitOfWork(IOptions<DbSettings> options)
    {
        _dbSettings = options.Value;
    }

    public NpgsqlConnection Connection => _connection;

    public DbTransaction Transaction => _transaction;
    
    private NpgsqlConnection EnsureConnection() =>
        _connection ??= (NpgsqlConnection) new NpgsqlConnection(_dbSettings.ConnectionString).EnsureOpen();
    
    public void Begin()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Cannot start a new transaction while the existing one is still open.");
        }

        _connection = EnsureConnection();
        _transaction = _connection.BeginTransaction();
    }
    
    public void Commit()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("There is no active transaction to commit.");
        }
        using (_transaction)
        {
            _transaction.Commit();
        }
        _transaction = null;
    }

    public void Rollback()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("There is no active transaction to rollback.");
        }
        using (_transaction)
        {
            _transaction.Rollback();
        }
        _transaction = null;
    }
}