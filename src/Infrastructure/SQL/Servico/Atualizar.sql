UPDATE servicos
SET
    nome = @Nome,
    codigo_interno = @CodigoInterno,
    descricao = @Descricao,
    preco = @Preco,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;