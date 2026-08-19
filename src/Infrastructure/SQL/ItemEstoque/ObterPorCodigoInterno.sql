SELECT
    id,
    codigo_interno,
    nome,
    descricao,
    tipo,
    preco,
    estoque,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
FROM itens_estoque
WHERE codigo_interno = @CodigoInterno;