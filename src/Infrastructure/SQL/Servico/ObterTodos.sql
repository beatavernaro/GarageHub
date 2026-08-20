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
WHERE ativo = TRUE
ORDER BY nome;