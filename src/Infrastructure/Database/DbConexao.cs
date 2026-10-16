using System.Data;

namespace Infrastructure.Database;

// Conexão entregue aos repositórios. Quando compartilhada com a Unit of Work,
// a conexão e a transação pertencem à sessão e não são confirmadas nem descartadas aqui.
public sealed class DbConexao(
    IDbConnection connection,
    IDbTransaction? transaction,
    bool compartilhada) : IDisposable
{
    private readonly bool _compartilhada = compartilhada;
    private bool _transacaoPropria;

    public IDbConnection Connection { get; } = connection;

    public IDbTransaction? Transaction { get; private set; } = transaction;

    // Inicia uma transação local somente quando não houver uma transação da Unit of Work
    public IDbTransaction IniciarTransacao()
    {
        if (Transaction is not null)
            return Transaction;

        if (Connection.State != ConnectionState.Open)
            Connection.Open();

        Transaction = Connection.BeginTransaction();
        _transacaoPropria = true;

        return Transaction;
    }

    public void Confirmar()
    {
        if (_transacaoPropria)
            Transaction!.Commit();
    }

    public void Desfazer()
    {
        if (_transacaoPropria)
            Transaction!.Rollback();
    }

    public void Dispose()
    {
        if (_transacaoPropria)
            Transaction?.Dispose();

        if (!_compartilhada)
            Connection.Dispose();
    }
}
