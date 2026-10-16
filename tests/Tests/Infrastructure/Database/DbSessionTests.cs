using Application.Interfaces;
using FluentAssertions;
using Infrastructure.Database;
using Moq;
using System.Data;

namespace GarageHub.Tests.Infrastructure.Database;

public class DbSessionTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly Mock<IDbTransaction> _transactionMock;

    private readonly DbSession _session;

    public DbSessionTests()
    {
        _connectionFactoryMock =
            new Mock<IDbConnectionFactory>();

        _connectionMock =
            new Mock<IDbConnection>();

        _transactionMock =
            new Mock<IDbTransaction>();

        _connectionMock
            .Setup(x => x.BeginTransaction())
            .Returns(_transactionMock.Object);

        _connectionFactoryMock
            .Setup(x => x.CreateConnection())
            .Returns(_connectionMock.Object);

        _session = new DbSession(
            _connectionFactoryMock.Object);
    }

    [Fact]
    public async Task ExecutarEmTransacaoAsync_Deve_Confirmar_Quando_Operacao_Concluir()
    {
        await _session.ExecutarEmTransacaoAsync(
            () => Task.CompletedTask);

        _transactionMock.Verify(
            x => x.Commit(),
            Times.Once);

        _transactionMock.Verify(
            x => x.Rollback(),
            Times.Never);

        _connectionMock.Verify(
            x => x.Dispose(),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarEmTransacaoAsync_Deve_Desfazer_Quando_Operacao_Falhar()
    {
        var acao =
            async () =>
                await _session.ExecutarEmTransacaoAsync(
                    () => throw new InvalidOperationException("Falha"));

        await acao.Should()
            .ThrowAsync<InvalidOperationException>();

        _transactionMock.Verify(
            x => x.Rollback(),
            Times.Once);

        _transactionMock.Verify(
            x => x.Commit(),
            Times.Never);
    }

    [Fact]
    public async Task ObterConexao_Deve_Compartilhar_Conexao_E_Transacao_Dentro_Da_Transacao()
    {
        await _session.ExecutarEmTransacaoAsync(() =>
        {
            using var primeira =
                _session.ObterConexao();

            using var segunda =
                _session.ObterConexao();

            primeira.Connection.Should()
                .BeSameAs(_connectionMock.Object);

            segunda.Connection.Should()
                .BeSameAs(_connectionMock.Object);

            primeira.Transaction.Should()
                .BeSameAs(_transactionMock.Object);

            // Transação local do repositório não deve abrir uma nova transação
            segunda.IniciarTransacao().Should()
                .BeSameAs(_transactionMock.Object);

            segunda.Confirmar();

            return Task.CompletedTask;
        });

        _connectionFactoryMock.Verify(
            x => x.CreateConnection(),
            Times.Once);

        _connectionMock.Verify(
            x => x.BeginTransaction(),
            Times.Once);

        // Somente a Unit of Work confirma e descarta a conexão
        _transactionMock.Verify(
            x => x.Commit(),
            Times.Once);

        _connectionMock.Verify(
            x => x.Dispose(),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarEmTransacaoAsync_Aninhada_Deve_Participar_Da_Transacao_Existente()
    {
        await _session.ExecutarEmTransacaoAsync(
            () => _session.ExecutarEmTransacaoAsync(
                () => Task.CompletedTask));

        _connectionMock.Verify(
            x => x.BeginTransaction(),
            Times.Once);

        _transactionMock.Verify(
            x => x.Commit(),
            Times.Once);
    }

    [Fact]
    public void ObterConexao_Fora_Da_Transacao_Deve_Criar_Conexao_Propria()
    {
        using (var conexao = _session.ObterConexao())
        {
            conexao.Transaction.Should().BeNull();
        }

        _connectionMock.Verify(
            x => x.Dispose(),
            Times.Once);
    }

    [Fact]
    public void IniciarTransacao_Fora_Da_Unit_Of_Work_Deve_Usar_Transacao_Local()
    {
        using (var conexao = _session.ObterConexao())
        {
            conexao.IniciarTransacao();
            conexao.Confirmar();
        }

        _connectionMock.Verify(
            x => x.Open(),
            Times.Once);

        _transactionMock.Verify(
            x => x.Commit(),
            Times.Once);

        _transactionMock.Verify(
            x => x.Dispose(),
            Times.Once);
    }
}
