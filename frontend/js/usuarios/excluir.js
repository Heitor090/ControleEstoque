const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');
const container = document.getElementById('dados-usuario');
const btnExcluir = document.getElementById('btn-excluir');

async function carregarExclusao() {
    if (!id) {
        container.innerHTML = '<p>ID não informado.</p>';
        btnExcluir.disabled = true;
        return;
    }

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Usuarios/${id}`);
        if (!response.ok) throw new Error('Usuário não encontrado');

        const u = await response.json();
        container.innerHTML = `
            <dl>
                <dt>ID:</dt><dd>${u.id}</dd>
                <dt>Nome:</dt><dd>${u.nome}</dd>
                <dt>E-mail:</dt><dd>${u.email}</dd>
                <dt>Perfil:</dt><dd>${u.perfil}</dd>
            </dl>
        `;
    } catch (error) {
        container.innerHTML = '<p>Erro ao carregar dados do usuário.</p>';
        btnExcluir.disabled = true;
    }
}

btnExcluir.addEventListener('click', async () => {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Usuarios/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Erro ao excluir');

        window.location.href = 'index.html';
    } catch (error) {
        alert('Erro ao excluir usuário.');
    }
});

carregarExclusao();
