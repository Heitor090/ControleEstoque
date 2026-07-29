const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

document.addEventListener('DOMContentLoaded', () => {
    buscarDetalhes();
});

async function buscarDetalhes() {
    const container = document.getElementById('dados-fornecedor');
    if (!container) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Fornecedores/${id}`);
        if (!response.ok) throw new Error('Erro ao carregar detalhes');

        const fornecedor = await response.json();

        container.innerHTML = `
            <p><strong>ID:</strong> ${fornecedor.id}</p>
            <p><strong>Nome Fantasia:</strong> ${fornecedor.nomeFantasia}</p>
            <p><strong>CNPJ:</strong> ${fornecedor.cnpj}</p>
        `;
    } catch (error) {
        container.innerHTML = `<p style="color: red;">Erro ao carregar detalhes do fornecedor.</p>`;
    }
}