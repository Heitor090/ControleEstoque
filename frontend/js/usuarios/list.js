document.addEventListener('DOMContentLoaded', () => {
    carregarUsuarios();
});

async function carregarUsuarios() {
    const feedback = document.getElementById('feedback-message');
    const tbody = document.getElementById('tabela-usuarios');
    if (!tbody) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Usuarios`);
        if (!response.ok) {
            throw new Error('Falha ao carregar usuários.');
        }
        const usuarios = await response.json();
        tbody.innerHTML = '';

        if (!Array.isArray(usuarios) || usuarios.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5">Nenhum usuário cadastrado.</td></tr>';
            if (feedback) feedback.textContent = '';
            return;
        }

        if (feedback) feedback.textContent = '';

        usuarios.forEach(usuario => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${usuario.id}</td>
                <td>${usuario.nome}</td>
                <td>${usuario.email}</td>
                <td>${usuario.perfil}</td>
                <td>
                    <a href="detalhes.html?id=${usuario.id}">Detalhes</a>
                    <a href="form.html?id=${usuario.id}">Editar</a>
                    <a href="excluir.html?id=${usuario.id}">Excluir</a>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error("Erro ao carregar usuários:", error);
        if (feedback) feedback.textContent = 'Erro ao carregar a lista de usuários.';
        tbody.innerHTML = '<tr><td colspan="5">Erro ao carregar usuários.</td></tr>';
    }
}
