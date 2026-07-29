document.addEventListener('DOMContentLoaded', () => {
    carregarFormasPagamento();
});

async function carregarFormasPagamento() {
    const feedback = document.getElementById('feedback-message');
    const tbody = document.getElementById('tabela-formaspagamento');
    if (!tbody) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/FormasPagamento`);
        if (!response.ok) {
            throw new Error('Falha ao carregar formas de pagamento.');
        }
        const formas = await response.json();
        tbody.innerHTML = '';

        if (!Array.isArray(formas) || formas.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4">Nenhuma forma de pagamento cadastrada.</td></tr>';
            if (feedback) feedback.textContent = '';
            return;
        }

        if (feedback) feedback.textContent = '';

        formas.forEach(forma => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${forma.id}</td>
                <td>${forma.nome}</td>
                <td>${forma.ativo ? 'Ativo' : 'Inativo'}</td>
                <td>
                    <a href="detalhes.html?id=${forma.id}">Detalhes</a>
                    <a href="form.html?id=${forma.id}">Editar</a>
                    <a href="excluir.html?id=${forma.id}">Excluir</a>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error("Erro ao carregar formas de pagamento:", error);
        if (feedback) feedback.textContent = 'Erro ao carregar as formas de pagamento.';
        tbody.innerHTML = '<tr><td colspan="4">Erro ao carregar formas de pagamento.</td></tr>';
    }
}
