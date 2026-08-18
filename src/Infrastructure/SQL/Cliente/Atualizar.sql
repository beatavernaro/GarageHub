UPDATE clientes
SET
    nome = @Nome,
    documento = @Documento,
    tipo_pessoa = @TipoPessoa,
    telefone = @Telefone,
    email = @Email,
    logradouro = @Logradouro,
    numero = @Numero,
    complemento = @Complemento,
    bairro = @Bairro,
    cidade = @Cidade,
    estado = @Estado,
    cep = @Cep,
    data_alteracao = @DataAlteracao,
    alterado_por_id = @AlteradoPorId,
    ativo = @Ativo
WHERE id = @Id;