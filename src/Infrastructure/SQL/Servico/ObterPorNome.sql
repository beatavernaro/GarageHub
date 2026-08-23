SELECT
    id,
    nome,
    descricao,
    preco,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
FROM servicos
WHERE nome = @Nome
  AND ativo = TRUE;