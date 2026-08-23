UPDATE ordens_servico
SET
    status = @Status,
    desconto = @Desconto,
    valor_total = @ValorTotal,
    data_inicio = @DataInicio,
    data_finalizacao = @DataFinalizacao,
    data_entrega = @DataEntrega,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;