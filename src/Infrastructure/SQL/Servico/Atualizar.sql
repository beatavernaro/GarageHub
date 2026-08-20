UPDATE servicos
SET
    nome = @Nome,
    descricao = @Descricao,
    preco = @Preco,
    status = @Status,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;