document.addEventListener('DOMContentLoaded', () => {
    carregarContas();
});

async function carregarContas() {
    const feedback = document.getElementById('feedback-message');
    const tbody = document.getElementById('tabela-contas');
    const titulo = document.getElementById('titulo-pagina');
    const btnNova = document.getElementById('btn-nova-conta');

    if (!tbody) return;

    const usuarioRaw = localStorage.getItem('usuario');
    const usuario = usuarioRaw ? JSON.parse(usuarioRaw) : null;
    const perfil = usuario?.perfil || 'Cliente';

    if (perfil === 'Cliente') {
        if (titulo) titulo.textContent = 'Minhas Contas a Pagar';
        if (btnNova) btnNova.style.display = 'none';
    } else {
        if (titulo) titulo.textContent = 'Contas a Receber';
        if (btnNova) btnNova.style.display = 'inline-block';
    }

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/ContasReceber`);
        if (!response.ok) throw new Error('Erro ao carregar contas');
        
        const contas = await response.json();
        tbody.innerHTML = '';

        if (!Array.isArray(contas) || contas.length === 0) {
            const msgVazia = (perfil === 'Cliente') 
                ? 'Nenhuma conta a pagar encontrada.' 
                : 'Nenhuma conta a receber cadastrada.';
            tbody.innerHTML = `<tr><td colspan="7">${msgVazia}</td></tr>`;
            if (feedback) feedback.textContent = '';
            return;
        }

        if (feedback) feedback.textContent = '';

        contas.forEach(conta => {
            const dataVencimento = new Date(conta.dataVencimento).toLocaleDateString('pt-BR');
            
            let acoesHtml = `<a href="detalhes.html?id=${conta.id}">Detalhes</a>`;
            if (perfil === 'Caixa' || perfil === 'Gerente') {
                acoesHtml += `
                    <a href="form.html?id=${conta.id}">Editar</a>
                    <a href="excluir.html?id=${conta.id}">Excluir</a>
                `;
            }

            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${conta.id}</td>
                <td>${conta.descricao}</td>
                <td>R$ ${conta.valor.toFixed(2)}</td>
                <td>${dataVencimento}</td>
                <td>${conta.status}</td>
                <td>${conta.cliente?.nome || 'Não informado'}</td>
                <td>${acoesHtml}</td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error("Erro ao carregar as contas:", error);
        if (feedback) feedback.textContent = 'Erro ao carregar lista de contas.';
        tbody.innerHTML = '<tr><td colspan="7">Erro ao carregar contas.</td></tr>';
    }
}
