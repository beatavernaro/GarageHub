INSERT INTO servicos (
    id,
    nome,
    descricao,
    preco,
    status,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
)
VALUES (
    @Id,
    @Nome,
    @Descricao,
    @Preco,
    @Status,
    @CriadoPorId,
    @DataCriacao,
    @DataAlteracao,
    @AlteradoPorId,
    @Ativo
);