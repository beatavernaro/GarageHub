namespace Application.Interfaces;

public interface IUnitOfWork
{
    // Executa a operação em uma única transação: todas as gravações
    // realizadas pelos repositórios são confirmadas juntas ou desfeitas juntas.
    Task ExecutarEmTransacaoAsync(Func<Task> operacao);
}
