const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('form-formapagamento');
    if (form) {
        form.addEventListener('submit', salvarFormaPagamento);
    }
    carregarFormaPagamento();
});

async function carregarFormaPagamento() {
    if (id) {
        const titulo = document.getElementById('titulo-pagina');
        if (titulo) titulo.innerText = 'Editar Forma de Pagamento';

        try {
            const response = await fetchWithToken(`${API_BASE_URL}/api/FormasPagamento/${id}`);
            if (!response.ok) throw new Error('Erro ao carregar forma de pagamento');

            const forma = await response.json();
            const inputNome = document.getElementById('nome');
            const selectAtivo = document.getElementById('ativo');
            if (inputNome) inputNome.value = forma.nome;
            if (selectAtivo) selectAtivo.value = forma.ativo ? "true" : "false";
        } catch (error) {
            console.error(error);
            const feedback = document.getElementById('feedback-message');
            if (feedback) feedback.textContent = 'Erro ao carregar dados da forma de pagamento.';
        }
    }
}

async function salvarFormaPagamento(e) {
    e.preventDefault();
    const feedback = document.getElementById('feedback-message');

    const inputNome = document.getElementById('nome');
    const selectAtivo = document.getElementById('ativo');
    if (!inputNome || !selectAtivo) return;

    const nome = inputNome.value.trim();
    const ativo = selectAtivo.value === "true";

    if (!nome) {
        if (feedback) feedback.textContent = 'Preencha o campo nome.';
        return;
    }

    const payload = { nome, ativo };
    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_BASE_URL}/api/FormasPagamento/${id}` : `${API_BASE_URL}/api/FormasPagamento`;

    try {
        const response = await fetchWithToken(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            const errData = await response.json().catch(() => ({}));
            throw new Error(errData.message || 'Erro ao salvar.');
        }

        window.location.href = 'index.html';
    } catch (error) {
        console.error(error);
        if (feedback) feedback.textContent = error.message;
    }
}
