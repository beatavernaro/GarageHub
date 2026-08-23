UPDATE ordens_servico_servicos
SET
    status = @Status,
    data_alteracao = NOW(),
    alterado_por_id = @AlteradoPorId
WHERE ordem_servico_id = @OrdemServicoId
  AND servico_id = @ServicoId
  AND ativo = TRUE;