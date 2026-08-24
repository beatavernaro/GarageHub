INSERT INTO servicos (
    id,
    codigo_interno,
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
    @CodigoInterno,
    @Nome,
    @Descricao,
    @Preco,
    @CriadoPorId,
    @DataCriacao,
    @DataAlteracao,
    @AlteradoPorId,
    @Ativo
);