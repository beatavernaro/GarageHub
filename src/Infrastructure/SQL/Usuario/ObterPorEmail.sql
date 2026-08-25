SELECT
    id AS Id,
    nome AS Nome,
    email AS Email,
    senha_hash AS SenhaHash,
    criado_por_id AS CriadoPorId,
    data_criacao AS DataCriacao,
    data_alteracao AS DataAlteracao,
    alterado_por_id AS AlteradoPorId,
    ativo AS Ativo
FROM usuarios
WHERE email = @Email
LIMIT 1;