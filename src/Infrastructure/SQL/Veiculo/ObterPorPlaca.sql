SELECT
    id,
    cliente_id,
    placa,
    chassi,
    marca,
    modelo,
    cor,
    ano,
    quilometragem,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
FROM veiculos
WHERE placa = @Placa;