UPDATE ordens_servico_servicos
SET
    status = @Status,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId
WHERE id = @Id
  AND ordem_servico_id = @OrdemServicoId
  AND ativo = TRUE;