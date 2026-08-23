INSERT INTO servicos (
    id,
    nome,
    descricao,
    preco,
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
    @CriadoPorId,
    @DataCriacao,
    @DataAlteracao,
    @AlteradoPorId,
    @Ativo
);