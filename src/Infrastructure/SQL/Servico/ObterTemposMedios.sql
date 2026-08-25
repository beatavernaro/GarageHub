SELECT
    oss.servico_id AS ServicoId,
    s.codigo_interno AS CodigoInterno,
    s.nome AS NomeServico,

    COUNT(*)::INTEGER AS QuantidadeExecucoes,

    AVG(
        EXTRACT(
            EPOCH FROM (
                oss.data_finalizacao -
                oss.data_inicio
            )
        )
    )::DOUBLE PRECISION AS TempoMedioSegundos

FROM ordens_servico_servicos oss

INNER JOIN servicos s
    ON s.id = oss.servico_id

WHERE oss.data_inicio IS NOT NULL
  AND oss.data_finalizacao IS NOT NULL
  AND oss.ativo = TRUE

GROUP BY
    oss.servico_id,
    s.codigo_interno,
    s.nome

ORDER BY s.nome ASC;