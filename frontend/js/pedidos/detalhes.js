const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');
const container = document.getElementById('dados-pedido');
const tbodyItens = document.getElementById('tabela-itens');

async function carregarDetalhes() {
    if (!id) {
        container.innerHTML = '<p>ID não informado.</p>';
        return;
    }

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Pedidos/${id}`);
        if (!response.ok) throw new Error('Pedido não encontrado');

        const pedido = await response.json();
        const dataFmt = new Date(pedido.dataPedido).toLocaleString('pt-BR');

        container.innerHTML = `
            <dl>
                <dt>ID:</dt><dd>${pedido.id}</dd>
                <dt>Data:</dt><dd>${dataFmt}</dd>
                <dt>Status:</dt><dd>${pedido.status}</dd>
                <dt>Pagamento:</dt><dd>${pedido.formaPagamentoNome || 'Não informada'}</dd>
            </dl>
        `;

        tbodyItens.innerHTML = '';
        let total = 0;

        (pedido.itens || []).forEach(item => {
            const subtotal = item.quantidade * item.precoUnitario;
            total += subtotal;
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${item.produtoNome || 'Produto ' + item.produtoId}</td>
                <td>${item.quantidade}</td>
                <td>R$ ${item.precoUnitario.toFixed(2)}</td>
                <td>R$ ${subtotal.toFixed(2)}</td>
            `;
            tbodyItens.appendChild(tr);
        });

        const trTotal = document.createElement('tr');
        trTotal.innerHTML = `
            <td colspan="3"><strong>Total</strong></td>
            <td><strong>R$ ${total.toFixed(2)}</strong></td>
        `;
        tbodyItens.appendChild(trTotal);

    } catch (error) {
        container.innerHTML = '<p>Erro ao carregar detalhes do pedido.</p>';
    }
}

carregarDetalhes();
