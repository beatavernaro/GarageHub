SELECT
    id AS OrdemServicoId,
    data_inicio AS DataInicio,
    data_finalizacao AS DataFinalizacao
FROM ordens_servico
WHERE data_inicio IS NOT NULL
  AND data_finalizacao IS NOT NULL
ORDER BY data_finalizacao DESC;