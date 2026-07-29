document.addEventListener('DOMContentLoaded', () => {
    carregarPedidos();
});

async function carregarPedidos() {
    const feedback = document.getElementById('feedback-message');
    const usuarioRaw = localStorage.getItem('usuario');
    const usuario = usuarioRaw ? JSON.parse(usuarioRaw) : null;
    const perfil = usuario?.perfil || 'Cliente';

    // Trata exibição do Carrinho em Cache
    const secaoCache = document.getElementById('secao-carrinho-cache');
    const tbodyCache = document.getElementById('tabela-carrinho-cache');

    if (perfil === 'Cliente') {
        if (secaoCache) secaoCache.style.display = 'block';
        renderizarCarrinhoCache(tbodyCache);
    } else {
        if (secaoCache) secaoCache.style.display = 'none';
    }

    const tituloAbertos = document.getElementById('titulo-pedidos-abertos');
    if (tituloAbertos) {
        tituloAbertos.textContent = (perfil === 'Cliente') 
            ? 'Meus Pedidos em Aberto' 
            : 'Pedidos a Processar (Aguardando Atendimento)';
    }

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Pedidos`);
        if (!response.ok) {
            throw new Error('Falha ao carregar lista de pedidos.');
        }

        const pedidos = await response.json();
        renderizarPedidosAbertos(pedidos.filter(p => p.status === 'Aberto'), perfil);
        renderizarPedidosFinalizados(pedidos.filter(p => p.status !== 'Aberto'), perfil);

    } catch (error) {
        console.error('Erro ao carregar pedidos:', error);
        if (feedback) feedback.textContent = 'Erro ao carregar a lista de pedidos.';
    }
}

function renderizarCarrinhoCache(tbody) {
    if (!tbody) return;
    tbody.innerHTML = '';
    const carrinho = (typeof obterCarrinho === 'function') ? obterCarrinho() : [];

    if (carrinho.length === 0) {
        tbody.innerHTML = '<tr><td colspan="4">Nenhum item pendente no carrinho.</td></tr>';
        return;
    }

    carrinho.forEach(item => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${item.nome}</td>
            <td>R$ ${item.preco.toFixed(2)}</td>
            <td>${item.quantidade}</td>
            <td>R$ ${(item.preco * item.quantidade).toFixed(2)}</td>
        `;
        tbody.appendChild(tr);
    });
}

function renderizarPedidosAbertos(pedidosAbertos, perfil) {
    const tbody = document.getElementById('tabela-pedidos-abertos');
    if (!tbody) return;
    tbody.innerHTML = '';

    if (pedidosAbertos.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7">Nenhum pedido em aberto.</td></tr>';
        return;
    }

    pedidosAbertos.forEach(pedido => {
        const dataFmt = new Date(pedido.dataPedido).toLocaleDateString('pt-BR');
        const tr = document.createElement('tr');

        let acoesHtml = `<a href="detalhes.html?id=${pedido.id}">Detalhes</a>`;
        if (perfil === 'Caixa' || perfil === 'Gerente') {
            acoesHtml += ` | <button type="button" onclick="finalizarPedido(${pedido.id})">Finalizar Pedido</button>`;
        }

        tr.innerHTML = `
            <td>${pedido.id}</td>
            <td>${dataFmt}</td>
            <td>${pedido.clienteNome || 'Cliente não informado'}</td>
            <td>${pedido.formaPagamentoNome || 'Não informada'}</td>
            <td>R$ ${pedido.total.toFixed(2)}</td>
            <td>${pedido.status}</td>
            <td>${acoesHtml}</td>
        `;
        tbody.appendChild(tr);
    });
}

function renderizarPedidosFinalizados(pedidosFinalizados, perfil) {
    const tbody = document.getElementById('tabela-pedidos-finalizados');
    if (!tbody) return;
    tbody.innerHTML = '';

    if (pedidosFinalizados.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8">Nenhum pedido finalizado.</td></tr>';
        return;
    }

    pedidosFinalizados.forEach(pedido => {
        const dataFmt = new Date(pedido.dataPedido).toLocaleDateString('pt-BR');
        const tr = document.createElement('tr');

        tr.innerHTML = `
            <td>${pedido.id}</td>
            <td>${dataFmt}</td>
            <td>${pedido.clienteNome || 'Cliente não informado'}</td>
            <td>${pedido.formaPagamentoNome || 'Não informada'}</td>
            <td>R$ ${pedido.total.toFixed(2)}</td>
            <td>${pedido.caixaNome || 'Atendido no Caixa'}</td>
            <td>${pedido.status}</td>
            <td><a href="detalhes.html?id=${pedido.id}">Detalhes</a></td>
        `;
        tbody.appendChild(tr);
    });
}

async function finalizarPedido(id) {
    if (!confirm(`Deseja finalizar o pedido #${id}?`)) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Pedidos/${id}/finalizar`, {
            method: 'PUT'
        });

        if (response.ok) {
            alert(`Pedido #${id} finalizado com sucesso!`);
            carregarPedidos();
        } else {
            const errData = await response.json().catch(() => ({}));
            alert(errData.message || 'Erro ao finalizar o pedido.');
        }
    } catch (err) {
        console.error('Erro ao finalizar pedido:', err);
        alert('Erro ao processar finalização.');
    }
}
