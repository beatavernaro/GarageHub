INSERT INTO servicos_itens_estoque (
    id,
    servico_id,
    item_estoque_id,
    quantidade,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
)
VALUES (
    @Id,
    @ServicoId,
    @ItemEstoqueId,
    @Quantidade,
    @CriadoPorId,
    @DataCriacao,
    @DataAlteracao,
    @AlteradoPorId,
    @Ativo
);