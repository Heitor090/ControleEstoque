const form = document.getElementById('form-pedido');
const feedback = document.getElementById('feedback-message');

async function carregarOpcoes() {
    try {
        const [resFormas, resProdutos] = await Promise.all([
            fetchWithToken(`${API_BASE_URL}/api/FormasPagamento`),
            fetchWithToken(`${API_BASE_URL}/api/Produtos`)
        ]);

        if (resFormas.ok) {
            const formas = await resFormas.json();
            const selectFormas = document.getElementById('formaPagamentoId');
            selectFormas.innerHTML = '<option value="">Selecione uma forma de pagamento</option>';
            formas.filter(f => f.ativo).forEach(f => {
                const opt = document.createElement('option');
                opt.value = f.id;
                opt.textContent = f.nome;
                selectFormas.appendChild(opt);
            });
        }

        if (resProdutos.ok) {
            const produtos = await resProdutos.json();
            const selectProdutos = document.getElementById('produtoId');
            selectProdutos.innerHTML = '<option value="">Selecione um produto</option>';
            produtos.forEach(p => {
                const opt = document.createElement('option');
                opt.value = p.id;
                opt.textContent = `${p.nome} - R$ ${p.preco.toFixed(2)} (Estoque: ${p.quantidadeEstoque})`;
                selectProdutos.appendChild(opt);
            });
        }
    } catch (error) {
        console.error(error);
        if (feedback) feedback.textContent = 'Erro ao carregar opções para o pedido.';
    }
}

form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const formaPagamentoId = parseInt(document.getElementById('formaPagamentoId').value, 10);
    const produtoId = parseInt(document.getElementById('produtoId').value, 10);
    const quantidade = parseInt(document.getElementById('quantidade').value, 10);

    if (!formaPagamentoId || !produtoId || !quantidade || quantidade <= 0) {
        if (feedback) feedback.textContent = 'Preencha todos os campos corretamente.';
        return;
    }

    const payload = {
        formaPagamentoId: formaPagamentoId,
        itens: [
            { produtoId: produtoId, quantidade: quantidade }
        ]
    };

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Pedidos`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            const errData = await response.json().catch(() => ({}));
            throw new Error(errData.message || 'Erro ao realizar pedido.');
        }

        window.location.href = 'index.html';
    } catch (error) {
        console.error(error);
        if (feedback) feedback.textContent = error.message;
    }
});

carregarOpcoes();
