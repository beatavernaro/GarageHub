UPDATE orcamentos
SET
    status = @Status,
    desconto = @Desconto,
    valor_total = @ValorTotal,
    data_envio_cliente = @DataEnvioCliente,
    data_aprovacao = @DataAprovacao,
    data_rejeicao = @DataRejeicao,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;