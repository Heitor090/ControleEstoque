const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

document.addEventListener('DOMContentLoaded', () => {
    const btnExcluir = document.getElementById('btn-excluir');
    if (btnExcluir) {
        btnExcluir.addEventListener('click', excluirFornecedor);
    }
    buscarDetalhes();
});

async function buscarDetalhes() {
    const container = document.getElementById('dados-fornecedor');
    if (!container) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Fornecedores/${id}`);
        if (!response.ok) throw new Error('Erro ao carregar fornecedor');
        
        const fornecedor = await response.json();

        container.innerHTML = `
            <h3>${fornecedor.nomeFantasia}</h3>
            <p><strong>CNPJ:</strong> ${fornecedor.cnpj}</p>
        `;
    } catch (error) {
        console.error("Erro ao carregar detalhes:", error);
        container.innerHTML = `<p style="color: red;">Erro ao carregar dados do fornecedor.</p>`;
    }
}

async function excluirFornecedor() {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Fornecedores/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Erro ao excluir fornecedor');
        
        window.location.href = 'index.html';
    } catch (error) {
        console.error("Erro ao excluir:", error);
        alert('Erro ao excluir o fornecedor. Tente novamente.');
    }
}