document.addEventListener('DOMContentLoaded', async () => {
    await carregarFormasPagamento();
    renderizarCarrinho();

    const form = document.getElementById('form-finalizar-pedido');
    if (form) {
        form.addEventListener('submit', gravarPedido);
    }
});

async function carregarFormasPagamento() {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/FormasPagamento`);
        if (!response.ok) return;

        const formas = await response.json();
        const select = document.getElementById('formaPagamentoId');
        select.innerHTML = '<option value="">Selecione uma forma de pagamento</option>';

        formas.filter(f => f.ativo).forEach(forma => {
            const option = document.createElement('option');
            option.value = forma.id;
            option.textContent = forma.nome;
            select.appendChild(option);
        });
    } catch (err) {
        console.error('Erro ao carregar formas de pagamento:', err);
    }
}

function renderizarCarrinho() {
    const carrinho = obterCarrinho();
    const tbody = document.getElementById('tabela-carrinho');
    const totalEl = document.getElementById('total-carrinho');
    const btnGravar = document.getElementById('btn-gravar-pedido');

    if (!tbody) return;
    tbody.innerHTML = '';

    if (carrinho.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5">Seu carrinho está vazio.</td></tr>';
        if (totalEl) totalEl.textContent = '0,00';
        if (btnGravar) btnGravar.disabled = true;
        return;
    }

    if (btnGravar) btnGravar.disabled = false;
    let total = 0;

    carrinho.forEach(item => {
        const subtotal = item.preco * item.quantidade;
        total += subtotal;

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${item.nome}</td>
            <td>R$ ${item.preco.toFixed(2)}</td>
            <td>
                <div class="qtd-control">
                    <button type="button" class="btn-sm" onclick="alterarQtd(${item.id}, -1)">-</button>
                    <span>${item.quantidade}</span>
                    <button type="button" class="btn-sm" onclick="alterarQtd(${item.id}, 1)">+</button>
                </div>
            </td>
            <td>R$ ${subtotal.toFixed(2)}</td>
            <td>
                <button type="button" onclick="removerItem(${item.id})">Remover</button>
            </td>
        `;
        tbody.appendChild(tr);
    });

    if (totalEl) totalEl.textContent = total.toFixed(2);
}

function alterarQtd(id, delta) {
    alterarQuantidadeCarrinho(id, delta);
    renderizarCarrinho();
}

function removerItem(id) {
    removerDoCarrinho(id);
    renderizarCarrinho();
}

function limparEAtualizar() {
    limparCarrinho();
    renderizarCarrinho();
}

async function gravarPedido(e) {
    e.preventDefault();

    const carrinho = obterCarrinho();
    if (carrinho.length === 0) {
        alert('Seu carrinho está vazio.');
        return;
    }

    const formaPagamentoId = parseInt(document.getElementById('formaPagamentoId').value, 10);
    if (!formaPagamentoId || isNaN(formaPagamentoId)) {
        alert('Selecione uma forma de pagamento.');
        return;
    }

    const pedidoData = {
        formaPagamentoId: formaPagamentoId,
        itens: carrinho.map(item => ({
            produtoId: item.id,
            quantidade: item.quantidade
        }))
    };

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Pedidos`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(pedidoData)
        });

        if (response.ok) {
            alert('Pedido gravado com sucesso!');
            limparCarrinho();
            window.location.href = '../pedidos/index.html';
        } else {
            const errData = await response.json().catch(() => ({}));
            alert(errData.message || 'Erro ao gravar pedido.');
        }
    } catch (err) {
        console.error('Erro ao gravar pedido:', err);
        alert('Erro ao gravar pedido. Tente novamente.');
    }
}
