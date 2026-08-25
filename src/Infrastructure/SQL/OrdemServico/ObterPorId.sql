SELECT
    id AS Id,
    orcamento_id AS OrcamentoId,
    cliente_id AS ClienteId,
    veiculo_id AS VeiculoId,
    status AS Status,
    desconto AS Desconto,
    valor_total AS ValorTotal,
    data_inicio AS DataInicio,
    data_finalizacao AS DataFinalizacao,
    data_entrega AS DataEntrega,
    criado_por_id AS CriadoPorId,
    data_criacao AS DataCriacao,
    data_alteracao AS DataAlteracao,
    alterado_por_id AS AlteradoPorId,
    ativo AS Ativo
FROM ordens_servico
WHERE id = @Id;

SELECT
    id AS Id,
    ordem_servico_id AS OrdemServicoId,
    servico_id AS ServicoId,
    nome_servico AS NomeServico,
    descricao_servico AS DescricaoServico,
    quantidade AS Quantidade,
    valor_unitario AS ValorUnitario,
    valor_total AS ValorTotal,
    status AS Status,
    data_inicio AS DataInicio,
    data_finalizacao AS DataFinalizacao,
    criado_por_id AS CriadoPorId,
    data_criacao AS DataCriacao,
    data_alteracao AS DataAlteracao,
    alterado_por_id AS AlteradoPorId,
    ativo AS Ativo
FROM ordens_servico_servicos
WHERE ordem_servico_id = @Id;

SELECT
    id AS Id,
    ordem_servico_id AS OrdemServicoId,
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
FROM ordens_servico_itens_estoque
WHERE ordem_servico_id = @Id;