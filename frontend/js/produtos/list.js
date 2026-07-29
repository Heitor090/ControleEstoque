document.addEventListener('DOMContentLoaded', () => {
    carregarProdutos();
});

async function carregarProdutos() {
    const feedback = document.getElementById('feedback-message');
    const tbody = document.getElementById('tabela-produtos');
    if (!tbody) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Produtos`);
        if (!response.ok) {
            throw new Error('Falha ao carregar os produtos.');
        }
        const produtos = await response.json();
        tbody.innerHTML = '';

        if (!Array.isArray(produtos) || produtos.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6">Nenhum produto cadastrado.</td></tr>';
            if (feedback) feedback.textContent = '';
            return;
        }

        if (feedback) feedback.textContent = '';

        produtos.forEach(produto => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${produto.id}</td>
                <td>${produto.nome}</td>
                <td>R$ ${produto.preco.toFixed(2)}</td>
                <td>${produto.quantidadeEstoque}</td>
                <td>${produto.fornecedor?.nomeFantasia || 'Não informado'}</td>
                <td>
                    <a href="detalhes.html?id=${produto.id}">Detalhes</a>
                    <a href="form.html?id=${produto.id}">Editar</a>
                    <a href="excluir.html?id=${produto.id}">Excluir</a>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error("Erro ao carregar os produtos:", error);
        if (feedback) feedback.textContent = 'Erro ao carregar a lista de produtos.';
        tbody.innerHTML = '<tr><td colspan="6">Erro ao carregar os produtos.</td></tr>';
    }
}