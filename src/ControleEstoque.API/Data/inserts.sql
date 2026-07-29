USE CE_DB;
GO

-- ========================================
-- FORNECEDORES
-- ========================================
INSERT INTO Fornecedores (NomeFantasia, CNPJ)
VALUES 
('Distribuidora Gama', '12345678000199'),
('Loja Tech', '98765432000188'),
('TechSupply Brasil', '45678901000156'),
('Eletrônicos Premium', '78901234000112'),
('Importadora Global', '11223344000177');

-- ========================================
-- PRODUTOS
-- ========================================
INSERT INTO Produtos (Nome, Preco, QauntidadeEstoque, FornecedorId)
VALUES
('Teclado Mecanico RGB', 250.00, 50, 1),
('Mouse Sem Fio', 120.50, 150, 2),
('Monitor 24 Polegadas 4K', 950.00, 20, 1),
('Monitor 27 Polegadas 4K', 1250.00, 15, 2),
('Headset Gamer Wireless', 350.75, 40, 3),
('Webcam Full HD 1080p', 280.00, 60, 3),
('SSD 1TB NVMe', 450.00, 100, 4),
('SSD 2TB NVMe', 850.00, 45, 4),
('Mousepad Grande RGB', 89.90, 200, 5),
('Hub USB-C 7 Portas', 199.99, 75, 5);

-- ========================================
-- USUARIOS (COMENTADO - Ver nota acima)
-- ========================================
Para testes, configure usuários via API ou use a interface administrativa
INSERT INTO Usuarios (Nome, Email, SenhaHash, Perfil, Discriminator, Turno, CPF, Setor)
VALUES
('João Silva Cliente', 'joao.cliente@email.com', '$2a$12$R9h/cIPz0gi.URNNGUQ2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUga', 0, 'Cliente', NULL, '12345678901', NULL),
('Maria Santos Cliente', 'maria.cliente@email.com', '$2a$12$R9h/cIPz0gi.URNNGUQ2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUga', 0, 'Cliente', NULL, '98765432109', NULL),
('Pedro Oliveira Caixa', 'pedro.caixa@email.com', '$2a$12$R9h/cIPz0gi.URNNGUQ2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUga', 1, 'Caixa', 'Manhã', NULL, NULL),
('Ana Costa Caixa', 'ana.caixa@email.com', '$2a$12$R9h/cIPz0gi.URNNGUQ2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUga', 1, 'Caixa', 'Tarde', NULL, NULL),
('Carlos Gerente', 'carlos.gerente@email.com', '$2a$12$R9h/cIPz0gi.URNNGUQ2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUga', 2, 'Gerente', NULL, NULL, 'Vendas Gerais'),
('Fernanda Gerente', 'fernanda.gerente@email.com', '$2a$12$R9h/cIPz0gi.URNNGUQ2OPST9/PgBkqquzi.Ss7KIUgO2t0jWMUga', 2, 'Gerente', NULL, NULL, 'Estoque e Logística');

-- ========================================
-- PEDIDOS (Comentados - Dependem de usuários)
-- ========================================
INSERT INTO Pedidos (DataPedido, Status, CaixaId, ClienteId)
VALUES
(GETDATE(), 'Aberto', 3, 1),
(DATEADD(DAY, -1, GETDATE()), 'Finalizado', 3, 1),
(DATEADD(DAY, -2, GETDATE()), 'Cancelado', 4, 2),
(DATEADD(DAY, -3, GETDATE()), 'Finalizado', 3, 2),
(DATEADD(DAY, -5, GETDATE()), 'Aberto', 4, 1);

-- ========================================
-- ITENS DO PEDIDO (Comentados - Dependem de pedidos)
-- ========================================
-- Pedido 1 - Aberto
INSERT INTO ItensPedido (Quantidade, PrecoUnitario, PedidoId, ProdutoId)
VALUES
(1, 250.00, 1, 1), -- Teclado
(2, 120.50, 1, 2), -- Mouse
(1, 350.75, 1, 5); -- Headset

-- Pedido 2 - Finalizado
INSERT INTO ItensPedido (Quantidade, PrecoUnitario, PedidoId, ProdutoId)
VALUES
(1, 950.00, 2, 3),  -- Monitor 24"
(1, 280.00, 2, 6);  -- Webcam

-- Pedido 3 - Cancelado
INSERT INTO ItensPedido (Quantidade, PrecoUnitario, PedidoId, ProdutoId)
VALUES
(2, 450.00, 3, 7),  -- SSD 1TB
(1, 89.90, 3, 9);   -- Mousepad

-- Pedido 4 - Finalizado
INSERT INTO ItensPedido (Quantidade, PrecoUnitario, PedidoId, ProdutoId)
VALUES
(1, 1250.00, 4, 4), -- Monitor 27"
(1, 850.00, 4, 8);  -- SSD 2TB

-- Pedido 5 - Aberto
INSERT INTO ItensPedido (Quantidade, PrecoUnitario, PedidoId, ProdutoId)
VALUES
(3, 89.90, 5, 9),   -- Mousepad
(2, 199.99, 5, 10); -- Hub USB