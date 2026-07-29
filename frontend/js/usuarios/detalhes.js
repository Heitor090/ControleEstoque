const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');
const container = document.getElementById('dados-usuario');

async function carregarDetalhes() {
    if (!id) {
        container.innerHTML = '<p>ID não informado.</p>';
        return;
    }

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Usuarios/${id}`);
        if (!response.ok) throw new Error('Usuário não encontrado');

        const u = await response.json();
        let especifico = '';
        if (u.cpf) especifico = `<dt>CPF:</dt><dd>${u.cpf}</dd>`;
        if (u.turno) especifico = `<dt>Turno:</dt><dd>${u.turno}</dd>`;
        if (u.setor) especifico = `<dt>Setor:</dt><dd>${u.setor}</dd>`;

        container.innerHTML = `
            <dl>
                <dt>ID:</dt><dd>${u.id}</dd>
                <dt>Nome:</dt><dd>${u.nome}</dd>
                <dt>E-mail:</dt><dd>${u.email}</dd>
                <dt>Perfil:</dt><dd>${u.perfil}</dd>
                ${especifico}
            </dl>
        `;
    } catch (error) {
        container.innerHTML = '<p>Erro ao carregar detalhes do usuário.</p>';
    }
}

carregarDetalhes();
