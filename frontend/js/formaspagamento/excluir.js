const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');
const container = document.getElementById('dados-formapagamento');
const btnExcluir = document.getElementById('btn-excluir');

async function carregarExclusao() {
    if (!id) {
        container.innerHTML = '<p>ID não informado.</p>';
        btnExcluir.disabled = true;
        return;
    }

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/FormasPagamento/${id}`);
        if (!response.ok) throw new Error('Não encontrada');

        const forma = await response.json();
        container.innerHTML = `
            <dl>
                <dt>ID:</dt><dd>${forma.id}</dd>
                <dt>Nome:</dt><dd>${forma.nome}</dd>
            </dl>
        `;
    } catch (error) {
        container.innerHTML = '<p>Erro ao carregar dados para exclusão.</p>';
        btnExcluir.disabled = true;
    }
}

btnExcluir.addEventListener('click', async () => {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/FormasPagamento/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Erro ao excluir');

        window.location.href = 'index.html';
    } catch (error) {
        alert('Erro ao excluir forma de pagamento.');
    }
});

carregarExclusao();
