UPDATE orcamentos_itens
SET
    ativo = FALSE,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId
WHERE id = @Id;