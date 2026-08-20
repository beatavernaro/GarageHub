UPDATE servicos_itens_estoque
SET
    quantidade = @Quantidade,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;