const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

async function buscarDetalhes() {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/ContasReceber/${id}`);
        if (!response.ok) throw new Error('Erro ao carregar conta');
        
        const conta = await response.json();

        document.getElementById('dados-conta').innerHTML = `
            <h3>${conta.descricao}</h3>
            <p><strong>Valor:</strong> R$ ${conta.valor.toFixed(2)}</p>
            <p><strong>Status:</strong> ${conta.status}</p>
        `;
    } catch (error) {
        console.error("Erro ao carregar detalhes:", error);
        document.getElementById('dados-conta').innerHTML = `<p style="color: red;">Erro ao carregar dados da conta.</p>`;
    }
}

document.getElementById('btn-excluir').addEventListener('click', async () => {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/ContasReceber/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Erro ao excluir conta');
        
        window.location.href = 'index.html';
    } catch (error) {
        console.error("Erro ao excluir:", error);
        alert('Erro ao excluir a conta a receber. Tente novamente.');
    }
});

buscarDetalhes();
