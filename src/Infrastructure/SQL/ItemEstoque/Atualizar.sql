UPDATE itens_estoque
SET
    codigo_interno = @CodigoInterno,
    nome = @Nome,
    descricao = @Descricao,
    tipo = @Tipo,
    preco = @Preco,
    estoque = @Estoque,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;