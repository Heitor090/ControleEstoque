const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

async function buscarDetalhes() {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Produtos/${id}`);
        if (!response.ok) throw new Error('Erro ao carregar produto');
        
        const produto = await response.json();
        
        document.getElementById('dados-produto').innerHTML = `
            <h3>${produto.nome}</h3>
            <p><strong>Preço:</strong> R$ ${produto.preco.toFixed(2)}</p>
        `;
    } catch (error) {
        console.error("Erro ao carregar detalhes:", error);
        document.getElementById('dados-produto').innerHTML = `<p style="color: red;">Erro ao carregar dados do produto.</p>`;
    }
}

document.getElementById('btn-excluir').addEventListener('click', async () => {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Produtos/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Erro ao excluir produto');
        
        window.location.href = 'index.html';
    } catch (error) {
        console.error("Erro ao excluir:", error);
        alert('Erro ao excluir o produto. Tente novamente.');
    }
});

buscarDetalhes();