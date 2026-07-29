const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

async function buscarDetalhes() {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Produtos/${id}`);
        const produto = await response.json();

        document.getElementById('dados-produto').innerHTML = `
            <p><strong>ID:</strong> ${produto.id}</p>
            <p><strong>Nome:</strong> ${produto.nome}</p>
            <p><strong>Preço:</strong> R$ ${produto.preco.toFixed(2)}</p>
            <p><strong>Quantidade em Estoque:</strong> ${produto.quantidadeEstoque}</p>
            <p><strong>Fornecedor:</strong> ${produto.fornecedor?.nomeFantasia || 'Não informado'}</p>
        `;
    } catch (error) {
        document.getElementById('dados-produto').innerHTML = `<p style="color: red;">Erro ao carregar detalhes.</p>`;
    }
}

buscarDetalhes();