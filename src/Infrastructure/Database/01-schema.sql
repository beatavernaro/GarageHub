-- ============================================
-- USUÁRIOS
-- ============================================

CREATE TABLE usuarios (
    id UUID PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    senha_hash VARCHAR(255) NOT NULL,
    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_usuarios_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_usuarios_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id)
);


-- ============================================
-- CLIENTES
-- ============================================

CREATE TABLE clientes (
    id UUID PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    documento VARCHAR(14) NOT NULL UNIQUE,
    tipo_pessoa INTEGER NOT NULL,
    telefone VARCHAR(11) NOT NULL,
    email VARCHAR(150) NOT NULL,

    -- Endereço
    logradouro VARCHAR(200),
    numero VARCHAR(20),
    complemento VARCHAR(100),
    bairro VARCHAR(100),
    cidade VARCHAR(100),
    estado VARCHAR(2),
    cep VARCHAR(8),

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_clientes_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_clientes_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_clientes_tipo_pessoa
        CHECK (tipo_pessoa IN (1, 2))
);


-- ============================================
-- VEÍCULOS
-- ============================================

CREATE TABLE veiculos (
    id UUID PRIMARY KEY,
    cliente_id UUID NOT NULL,
    placa VARCHAR(7) NOT NULL UNIQUE,
    chassi VARCHAR(17),
    marca VARCHAR(100) NOT NULL,
    modelo VARCHAR(100) NOT NULL,
    cor VARCHAR(50) NOT NULL,
    ano INTEGER NOT NULL,
    quilometragem INTEGER NOT NULL,

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_veiculos_cliente
        FOREIGN KEY (cliente_id)
        REFERENCES clientes(id),

    CONSTRAINT fk_veiculos_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_veiculos_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_veiculos_ano
        CHECK (ano > 0),

    CONSTRAINT ck_veiculos_quilometragem
        CHECK (quilometragem >= 0)
);


-- ============================================
-- ITENS DE ESTOQUE
-- ============================================

CREATE TABLE itens_estoque (
    id UUID PRIMARY KEY,
    codigo_interno VARCHAR(7) NOT NULL UNIQUE,
    nome VARCHAR(150) NOT NULL,
    descricao VARCHAR(500),
    tipo INTEGER NOT NULL,
    preco NUMERIC(12,2) NOT NULL,
    estoque INTEGER NOT NULL DEFAULT 0,

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_itens_estoque_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_itens_estoque_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_itens_estoque_tipo
        CHECK (tipo IN (1, 2)),

    CONSTRAINT ck_itens_estoque_preco
        CHECK (preco > 0),

    CONSTRAINT ck_itens_estoque_estoque
        CHECK (estoque >= 0),

    CONSTRAINT ck_itens_estoque_codigo
        CHECK (codigo_interno ~ '^[A-Z]{3}[0-9]{4}$')
);


-- ============================================
-- SERVIÇOS
-- ============================================

CREATE TABLE servicos (
    id UUID PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    descricao VARCHAR(500),
    preco NUMERIC(12,2) NOT NULL,

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_servicos_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_servicos_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_servicos_preco
        CHECK (preco > 0)
);


-- ============================================
-- ORÇAMENTOS
-- ============================================

CREATE TABLE orcamentos (
    id UUID PRIMARY KEY,
    cliente_id UUID NOT NULL,
    veiculo_id UUID NOT NULL,
    status INTEGER NOT NULL DEFAULT 0,
    desconto NUMERIC(12,2) NOT NULL DEFAULT 0,
    valor_total NUMERIC(12,2) NOT NULL DEFAULT 0,
    data_envio_cliente TIMESTAMPTZ,
    data_aprovacao TIMESTAMPTZ,
    data_rejeicao TIMESTAMPTZ,

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_orcamentos_cliente
        FOREIGN KEY (cliente_id)
        REFERENCES clientes(id),

    CONSTRAINT fk_orcamentos_veiculo
        FOREIGN KEY (veiculo_id)
        REFERENCES veiculos(id),

    CONSTRAINT fk_orcamentos_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_orcamentos_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_orcamentos_status
        CHECK (status BETWEEN 0 AND 5),

    CONSTRAINT ck_orcamentos_desconto
        CHECK (desconto >= 0),

    CONSTRAINT ck_orcamentos_valor_total
        CHECK (valor_total >= 0)
);


-- ============================================
-- ITENS DO ORÇAMENTO
-- ============================================

CREATE TABLE orcamentos_itens (
    id UUID PRIMARY KEY,
    orcamento_id UUID NOT NULL,
    servico_id UUID,
    item_estoque_id UUID,
    quantidade INTEGER NOT NULL,
    valor_unitario NUMERIC(12,2) NOT NULL,
    valor_total NUMERIC(12,2) NOT NULL,

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_orcamentos_itens_orcamento
        FOREIGN KEY (orcamento_id)
        REFERENCES orcamentos(id),

    CONSTRAINT fk_orcamentos_itens_servico
        FOREIGN KEY (servico_id)
        REFERENCES servicos(id),

    CONSTRAINT fk_orcamentos_itens_estoque
        FOREIGN KEY (item_estoque_id)
        REFERENCES itens_estoque(id),

    CONSTRAINT fk_orcamentos_itens_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_orcamentos_itens_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_orcamentos_itens_tipo
        CHECK (
            (servico_id IS NOT NULL AND item_estoque_id IS NULL)
            OR
            (servico_id IS NULL AND item_estoque_id IS NOT NULL)
        ),

    CONSTRAINT ck_orcamentos_itens_quantidade
        CHECK (quantidade > 0),

    CONSTRAINT ck_orcamentos_itens_valor_unitario
        CHECK (valor_unitario > 0),

    CONSTRAINT ck_orcamentos_itens_valor_total
        CHECK (valor_total > 0)
);


-- ============================================
-- ORDENS DE SERVIÇO
-- ============================================

CREATE TABLE ordens_servico (
    id UUID PRIMARY KEY,
    orcamento_id UUID NOT NULL,
    cliente_id UUID NOT NULL,
    veiculo_id UUID NOT NULL,
    status INTEGER NOT NULL DEFAULT 0,
    desconto NUMERIC(12,2) NOT NULL DEFAULT 0,
    valor_total NUMERIC(12,2) NOT NULL DEFAULT 0,
    data_inicio TIMESTAMPTZ,
    data_finalizacao TIMESTAMPTZ,
    data_entrega TIMESTAMPTZ,

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_ordens_servico_orcamento
        FOREIGN KEY (orcamento_id)
        REFERENCES orcamentos(id),

    CONSTRAINT fk_ordens_servico_cliente
        FOREIGN KEY (cliente_id)
        REFERENCES clientes(id),

    CONSTRAINT fk_ordens_servico_veiculo
        FOREIGN KEY (veiculo_id)
        REFERENCES veiculos(id),

    CONSTRAINT fk_ordens_servico_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_ordens_servico_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_ordens_servico_status
        CHECK (status BETWEEN 0 AND 3),

    CONSTRAINT ck_ordens_servico_desconto
        CHECK (desconto >= 0),

    CONSTRAINT ck_ordens_servico_valor_total
        CHECK (valor_total >= 0)
);


-- ============================================
-- ITENS DA ORDEM DE SERVIÇO
-- ============================================

CREATE TABLE ordens_servico_itens (
    id UUID PRIMARY KEY,
    ordem_servico_id UUID NOT NULL,
    servico_id UUID NOT NULL,

    -- SNAPSHOT DO SERVIÇO
    nome_servico VARCHAR(150) NOT NULL,
    descricao_servico VARCHAR(500),
    quantidade INTEGER NOT NULL,
    valor_unitario NUMERIC(12,2) NOT NULL,
    valor_total NUMERIC(12,2) NOT NULL,

    criado_por_id UUID NOT NULL,
    data_criacao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    alterado_por_id UUID,
    ativo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_ordens_servico_itens_ordem
        FOREIGN KEY (ordem_servico_id)
        REFERENCES ordens_servico(id),

    CONSTRAINT fk_ordens_servico_itens_servico
        FOREIGN KEY (servico_id)
        REFERENCES servicos(id),

    CONSTRAINT fk_ordens_servico_itens_criado_por
        FOREIGN KEY (criado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT fk_ordens_servico_itens_alterado_por
        FOREIGN KEY (alterado_por_id)
        REFERENCES usuarios(id),

    CONSTRAINT ck_ordens_servico_itens_quantidade
        CHECK (quantidade > 0),

    CONSTRAINT ck_ordens_servico_itens_valor_unitario
        CHECK (valor_unitario > 0),

    CONSTRAINT ck_ordens_servico_itens_valor_total
        CHECK (valor_total > 0)
);