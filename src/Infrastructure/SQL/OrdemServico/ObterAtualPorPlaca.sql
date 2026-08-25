WITH ordem_atual AS (
    SELECT
        os.id
    FROM ordens_servico os
    INNER JOIN veiculos v
        ON v.id = os.veiculo_id
    WHERE v.placa = @Placa
      AND os.ativo = TRUE
    ORDER BY os.data_criacao DESC
    LIMIT 1
)

SELECT
    os.id AS Id,
    os.orcamento_id AS OrcamentoId,
    os.cliente_id AS ClienteId,
    os.veiculo_id AS VeiculoId,
    os.status AS Status,
    os.desconto AS Desconto,
    os.valor_total AS ValorTotal,
    os.data_inicio AS DataInicio,
    os.data_finalizacao AS DataFinalizacao,
    os.data_entrega AS DataEntrega,
    os.criado_por_id AS CriadoPorId,
    os.data_criacao AS DataCriacao,
    os.data_alteracao AS DataAlteracao,
    os.alterado_por_id AS AlteradoPorId,
    os.ativo AS Ativo
FROM ordens_servico os
INNER JOIN ordem_atual oa
    ON oa.id = os.id;


WITH ordem_atual AS (
    SELECT
        os.id
    FROM ordens_servico os
    INNER JOIN veiculos v
        ON v.id = os.veiculo_id
    WHERE v.placa = @Placa
      AND os.ativo = TRUE
    ORDER BY os.data_criacao DESC
    LIMIT 1
)

SELECT
    oss.id AS Id,
    oss.ordem_servico_id AS OrdemServicoId,
    oss.servico_id AS ServicoId,
    oss.nome_servico AS NomeServico,
    oss.descricao_servico AS DescricaoServico,
    oss.quantidade AS Quantidade,
    oss.valor_unitario AS ValorUnitario,
    oss.valor_total AS ValorTotal,
    oss.status AS Status,
    oss.data_inicio AS DataInicio,
    oss.data_finalizacao AS DataFinalizacao,
    oss.criado_por_id AS CriadoPorId,
    oss.data_criacao AS DataCriacao,
    oss.data_alteracao AS DataAlteracao,
    oss.alterado_por_id AS AlteradoPorId,
    oss.ativo AS Ativo
FROM ordens_servico_servicos oss
INNER JOIN ordem_atual oa
    ON oa.id = oss.ordem_servico_id
WHERE oss.ativo = TRUE
ORDER BY oss.data_criacao;