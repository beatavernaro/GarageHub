using Application.Interfaces;
using System.Data;

namespace Infrastructure.Database;

// Sessão de banco por requisição. Enquanto uma transação da Unit of Work
// estiver ativa, os repositórios compartilham a mesma conexão e transação.
public sealed class DbSession(IDbConnectionFactory connectionFactory) : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory =
        connectionFactory;

    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public async Task ExecutarEmTransacaoAsync(Func<Task> operacao)
    {
        // Transação já em andamento: a operação participa dela
        if (_transaction is not null)
        {
            await operacao();
            return;
        }

        using var connection =
            _connectionFactory.CreateConnection();

        connection.Open();

        using var transaction =
            connection.BeginTransaction();

        _connection = connection;
        _transaction = transaction;

        try
        {
            await operacao();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            _connection = null;
            _transaction = null;
        }
    }

    public DbConexao ObterConexao()
    {
        return _transaction is not null
            ? new DbConexao(_connection!, _transaction, compartilhada: true)
            : new DbConexao(_connectionFactory.CreateConnection(), null, compartilhada: false);
    }
}
