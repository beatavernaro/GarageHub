-- ============================================
-- USUÁRIOS
-- ============================================

INSERT INTO usuarios (
    id,
    nome,
    email,
    senha_hash,
    criado_por_id,
    data_criacao,
    ativo
)
VALUES
(
    '00000000-0000-0000-0000-000000000001',
    'Administrador',
    'admin@garagehub.com',
    '$2a$11$abcdefghijklmnopqrstuv',
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
),
(
    '00000000-0000-0000-0000-000000000002',
    'Atendente',
    'atendente@garagehub.com',
    '$2a$11$abcdefghijklmnopqrstuv',
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- CLIENTES
-- ============================================

INSERT INTO clientes (
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
    ativo
)
VALUES
(
    '10000000-0000-0000-0000-000000000001',
    'João da Silva',
    '12345678901',
    1,
    '15999990001',
    'joao@email.com',
    'Rua das Flores',
    '100',
    NULL,
    'Centro',
    'Sorocaba',
    'SP',
    '18000000',
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
),
(
    '10000000-0000-0000-0000-000000000002',
    'Empresa XPTO LTDA',
    '12345678000199',
    2,
    '1533334444',
    'contato@xpto.com',
    'Avenida Brasil',
    '500',
    'Sala 2',
    'Centro',
    'Sorocaba',
    'SP',
    '18000001',
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- VEÍCULOS
-- ============================================

INSERT INTO veiculos (
    id,
    cliente_id,
    placa,
    chassi,
    marca,
    modelo,
    cor,
    ano,
    quilometragem,
    criado_por_id,
    data_criacao,
    ativo
)
VALUES
(
    '20000000-0000-0000-0000-000000000001',
    '10000000-0000-0000-0000-000000000001',
    'ABC1234',
    '9BWZZZ377VT004251',
    'Volkswagen',
    'Gol',
    'Prata',
    2020,
    45000,
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
),
(
    '20000000-0000-0000-0000-000000000002',
    '10000000-0000-0000-0000-000000000002',
    'DEF5678',
    '9BWZZZ377VT004252',
    'Chevrolet',
    'Onix',
    'Preto',
    2022,
    28000,
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- ITENS DE ESTOQUE
-- ============================================

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
    ativo
)
VALUES
(
    '30000000-0000-0000-0000-000000000001',
    'PEC0001',
    'Pastilha de Freio Dianteira',
    'Jogo de pastilhas de freio dianteiras',
    1,
    180.00,
    20,
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
),
(
    '30000000-0000-0000-0000-000000000002',
    'INS0001',
    'Óleo 5W30',
    'Óleo sintético para motor',
    2,
    45.00,
    50,
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- SERVIÇOS
-- ============================================

INSERT INTO servicos (
    id,
    nome,
    descricao,
    preco,
    criado_por_id,
    data_criacao,
    ativo
)
VALUES
(
    '40000000-0000-0000-0000-000000000001',
    'Troca de Pastilhas de Freio',
    'Substituição das pastilhas de freio dianteiras',
    250.00,
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
),
(
    '40000000-0000-0000-0000-000000000002',
    'Troca de Óleo',
    'Troca do óleo e verificação do nível',
    120.00,
    '00000000-0000-0000-0000-000000000001',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- ORÇAMENTOS
-- ============================================

INSERT INTO orcamentos (
    id,
    cliente_id,
    veiculo_id,
    status,
    desconto,
    valor_total,
    data_aprovacao,
    data_rejeicao,
    criado_por_id,
    data_criacao,
    ativo
)
VALUES
(
    '60000000-0000-0000-0000-000000000001',
    '10000000-0000-0000-0000-000000000001',
    '20000000-0000-0000-0000-000000000001',
    0,
    0.00,
    250.00,
    NULL,
    NULL,
    '00000000-0000-0000-0000-000000000002',
    NOW(),
    TRUE
),
(
    '60000000-0000-0000-0000-000000000002',
    '10000000-0000-0000-0000-000000000002',
    '20000000-0000-0000-0000-000000000002',
    2,
    20.00,
    350.00,
    NOW(),
    NULL,
    '00000000-0000-0000-0000-000000000002',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- ITENS DOS ORÇAMENTOS
-- ============================================

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
    ativo
)
VALUES
(
    '70000000-0000-0000-0000-000000000001',
    '60000000-0000-0000-0000-000000000001',
    '40000000-0000-0000-0000-000000000001',
    NULL,
    1,
    250.00,
    250.00,
    '00000000-0000-0000-0000-000000000002',
    NOW(),
    TRUE
),
(
    '70000000-0000-0000-0000-000000000002',
    '60000000-0000-0000-0000-000000000002',
    '40000000-0000-0000-0000-000000000002',
    NULL,
    1,
    120.00,
    120.00,
    '00000000-0000-0000-0000-000000000002',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- ORDENS DE SERVIÇO
-- ============================================

INSERT INTO ordens_servico (
    id,
    orcamento_id,
    cliente_id,
    veiculo_id,
    status,
    desconto,
    valor_total,
    data_inicio,
    data_finalizacao,
    data_entrega,
    criado_por_id,
    data_criacao,
    ativo
)
VALUES
(
    '80000000-0000-0000-0000-000000000001',
    '60000000-0000-0000-0000-000000000002',
    '10000000-0000-0000-0000-000000000002',
    '20000000-0000-0000-0000-000000000002',
    1,
    20.00,
    350.00,
    NOW(),
    NULL,
    NULL,
    '00000000-0000-0000-0000-000000000002',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;


-- ============================================
-- ITENS DAS ORDENS DE SERVIÇO
-- SNAPSHOT DO SERVIÇO
-- ============================================

INSERT INTO ordens_servico_itens (
    id,
    ordem_servico_id,
    servico_id,
    nome_servico,
    descricao_servico,
    quantidade,
    valor_unitario,
    valor_total,
    criado_por_id,
    data_criacao,
    ativo
)
VALUES
(
    '90000000-0000-0000-0000-000000000001',
    '80000000-0000-0000-0000-000000000001',
    '40000000-0000-0000-0000-000000000002',
    'Troca de Óleo',
    'Troca do óleo e verificação do nível',
    1,
    120.00,
    120.00,
    '00000000-0000-0000-0000-000000000002',
    NOW(),
    TRUE
)
ON CONFLICT (id) DO NOTHING;