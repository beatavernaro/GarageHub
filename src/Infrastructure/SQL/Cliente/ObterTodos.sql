SELECT
    id,
    nome,
    documento,
    tipo_pessoa,
    telefone,
    email,
    logradouro,
    numero,
    complemento,
    bairro,
    cidade,
    estado,
    cep,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
FROM clientes
WHERE ativo = TRUE
ORDER BY nome ASC;