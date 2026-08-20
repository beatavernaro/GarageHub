SELECT
    id,
    nome,
    descricao,
    preco,
    status,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
FROM servicos
WHERE id = @Id;

SELECT
    id,
    servico_id,
    item_estoque_id,
    quantidade,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
FROM servicos_itens_estoque
WHERE servico_id = @Id
  AND ativo = TRUE;