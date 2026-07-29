const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');
const container = document.getElementById('dados-formapagamento');

async function carregarDetalhes() {
    if (!id) {
        container.innerHTML = '<p>ID não especificado.</p>';
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
                <dt>Status:</dt><dd>${forma.ativo ? 'Ativo' : 'Inativo'}</dd>
            </dl>
        `;
    } catch (error) {
        container.innerHTML = '<p>Erro ao carregar detalhes.</p>';
    }
}

carregarDetalhes();
