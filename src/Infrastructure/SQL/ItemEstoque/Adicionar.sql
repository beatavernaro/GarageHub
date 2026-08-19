INSERT INTO itens_estoque (
    id,
    codigo_interno,
    nome,
    descricao,
    tipo,
    preco,
    estoque,
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
    @Tipo,
    @Preco,
    @Estoque,
    @CriadoPorId,
    @DataCriacao,
    @DataAlteracao,
    @AlteradoPorId,
    @Ativo
);