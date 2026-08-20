INSERT INTO orcamentos_itens (
    id,
    orcamento_id,
    servico_id,
    item_estoque_id,
    quantidade,
    valor_unitario,
    valor_total,
    criado_por_id,
    data_criacao,
    data_alteracao,
    alterado_por_id,
    ativo
)
VALUES (
    @Id,
    @OrcamentoId,
    @ServicoId,
    @ItemEstoqueId,
    @Quantidade,
    @ValorUnitario,
    @ValorTotal,
    @CriadoPorId,
    @DataCriacao,
    @DataAlteracao,
    @AlteradoPorId,
    @Ativo
);