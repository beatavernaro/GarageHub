SELECT
    id AS Id,
    cliente_id AS ClienteId,
    veiculo_id AS VeiculoId,
    status AS Status,
    desconto AS Desconto,
    valor_total AS ValorTotal,
    data_envio_cliente AS DataEnvioCliente,
    data_aprovacao AS DataAprovacao,
    data_rejeicao AS DataRejeicao,
    criado_por_id AS CriadoPorId,
    data_criacao AS DataCriacao,
    data_alteracao AS DataAlteracao,
    alterado_por_id AS AlteradoPorId,
    ativo AS Ativo
FROM orcamentos
ORDER BY data_criacao DESC;

SELECT
    id AS Id,
    orcamento_id AS OrcamentoId,
    servico_id AS ServicoId,
    item_estoque_id AS ItemEstoqueId,
    nome_item AS NomeItem,
    descricao_item AS DescricaoItem,
    quantidade AS Quantidade,
    valor_unitario AS ValorUnitario,
    valor_total AS ValorTotal,
    criado_por_id AS CriadoPorId,
    data_criacao AS DataCriacao,
    data_alteracao AS DataAlteracao,
    alterado_por_id AS AlteradoPorId,
    ativo AS Ativo
FROM orcamentos_itens
ORDER BY data_criacao;