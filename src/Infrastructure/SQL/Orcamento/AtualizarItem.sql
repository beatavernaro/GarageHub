UPDATE orcamentos_itens
SET
    quantidade = @Quantidade,
    valor_unitario = @ValorUnitario,
    valor_total = @ValorTotal,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId
WHERE id = @Id;