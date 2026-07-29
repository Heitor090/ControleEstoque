document.addEventListener('DOMContentLoaded', () => {
    carregarFornecedores();
});

async function carregarFornecedores() {
    const feedback = document.getElementById('feedback-message');
    const tbody = document.getElementById('tabela-fornecedores');
    if (!tbody) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Fornecedores`);
        if (!response.ok) {
            throw new Error('Falha ao carregar fornecedores.');
        }
        const fornecedores = await response.json();
        tbody.innerHTML = '';

        if (!Array.isArray(fornecedores) || fornecedores.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4">Nenhum fornecedor cadastrado.</td></tr>';
            if (feedback) feedback.textContent = '';
            return;
        }

        if (feedback) feedback.textContent = '';

        fornecedores.forEach(fornecedor => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${fornecedor.id}</td>
                <td>${fornecedor.nomeFantasia}</td>
                <td>${fornecedor.cnpj}</td>
                <td>
                    <a href="detalhes.html?id=${fornecedor.id}">Detalhes</a>
                    <a href="form.html?id=${fornecedor.id}">Editar</a>
                    <a href="excluir.html?id=${fornecedor.id}">Excluir</a>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error("Erro ao carregar os fornecedores:", error);
        if (feedback) feedback.textContent = 'Erro ao carregar a lista de fornecedores.';
        tbody.innerHTML = '<tr><td colspan="4">Erro ao carregar fornecedores.</td></tr>';
    }
}