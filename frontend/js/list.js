async function carregarFormasPagamento() {
    const feedback = document.getElementById('feedback-message');
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/FormasPagamento`);
        if (!response.ok) {
            throw new Error('Falha ao carregar as formas de pagamento. Status: ' + response.status);
        }
        const formas = await response.json();

        const tbody = document.getElementById('tabela-formas-pagamento');
        tbody.innerHTML = '';

        if (formas.length === 0) {
            feedback.textContent = 'Nenhuma forma de pagamento encontrada.';
            feedback.style.display = 'block';
            return;
        }

        formas.forEach(forma => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${forma.id}</td>
                <td>${forma.nome}</td>
                <td>${forma.ativo ? 'Ativo' : 'Inativo'}</td>
                <td class="actions">
                    <a href="form.html?id=${forma.id}">Editar</a>
                    <a href="#" onclick="excluirFormaPagamento(${forma.id})" style="color: var(--danger-color);">Excluir</a>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error("Erro ao carregar as formas de pagamento:", error);
        feedback.textContent = 'Erro ao carregar a lista de formas de pagamento. Tente novamente mais tarde.';
        feedback.style.display = 'block';
    }
}

carregarFormasPagamento();