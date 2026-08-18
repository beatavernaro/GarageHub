UPDATE veiculos
SET
    cliente_id = @ClienteId,
    placa = @Placa,
    chassi = @Chassi,
    marca = @Marca,
    modelo = @Modelo,
    cor = @Cor,
    ano = @Ano,
    quilometragem = @Quilometragem,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;