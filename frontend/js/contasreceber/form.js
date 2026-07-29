const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('form-conta');
    if (form) {
        form.addEventListener('submit', salvarConta);
    }
    carregarConta();
});

async function carregarClientes() {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Usuarios`);
        if (!response.ok) throw new Error('Erro ao carregar clientes');

        const usuarios = await response.json();
        const select = document.getElementById('clienteId');
        if (!select) return;

        select.innerHTML = '<option value="">Selecione um cliente</option>';

        usuarios
            .filter(usuario => usuario.perfil === 'Cliente')
            .forEach(cliente => {
                const option = document.createElement('option');
                option.value = cliente.id;
                option.textContent = cliente.nome;
                select.appendChild(option);
            });
    } catch (error) {
        console.error('Erro ao carregar clientes:', error);
        const feedback = document.getElementById('feedback-message');
        if (feedback) feedback.textContent = 'Erro ao carregar a lista de clientes.';
    }
}

async function carregarConta() {
    await carregarClientes();

    if (id) {
        const titulo = document.getElementById('titulo-pagina');
        if (titulo) titulo.innerText = 'Editar Conta a Receber';

        try {
            const response = await fetchWithToken(`${API_BASE_URL}/api/ContasReceber/${id}`);
            if (!response.ok) throw new Error('Erro ao carregar conta');

            const conta = await response.json();

            const inputDesc = document.getElementById('descricao');
            const inputValor = document.getElementById('valor');
            const inputVenc = document.getElementById('dataVencimento');
            const inputPag = document.getElementById('dataPagamento');
            const selectStatus = document.getElementById('status');
            const selectCliente = document.getElementById('clienteId');

            if (inputDesc) inputDesc.value = conta.descricao;
            if (inputValor) inputValor.value = conta.valor;
            if (inputVenc) inputVenc.value = formatarDataParaInput(conta.dataVencimento);
            if (inputPag && conta.dataPagamento) inputPag.value = formatarDataParaInput(conta.dataPagamento);
            if (selectStatus) selectStatus.value = conta.status;
            if (selectCliente) selectCliente.value = conta.clienteId;
        } catch (error) {
            console.error('Erro ao carregar conta:', error);
            const feedback = document.getElementById('feedback-message');
            if (feedback) feedback.textContent = 'Erro ao carregar os dados da conta.';
        }
    }
}

function formatarDataParaInput(dataISO) {
    const data = new Date(dataISO);
    const dia = String(data.getDate()).padStart(2, '0');
    const mes = String(data.getMonth() + 1).padStart(2, '0');
    const ano = data.getFullYear();
    return `${ano}-${mes}-${dia}`;
}

async function salvarConta(e) {
    e.preventDefault();

    const inputDesc = document.getElementById('descricao');
    const inputValor = document.getElementById('valor');
    const inputVenc = document.getElementById('dataVencimento');
    const inputPag = document.getElementById('dataPagamento');
    const selectStatus = document.getElementById('status');
    const selectCliente = document.getElementById('clienteId');

    if (!inputDesc || !inputValor || !inputVenc || !selectStatus || !selectCliente) return;

    const descricao = inputDesc.value.trim();
    const valor = parseFloat(inputValor.value);
    const dataVencimento = inputVenc.value;
    const dataPagamento = inputPag ? inputPag.value : null;
    const status = selectStatus.value;
    const clienteId = parseInt(selectCliente.value, 10);

    if (!descricao || Number.isNaN(valor) || valor < 0 || !dataVencimento || !status || Number.isNaN(clienteId) || clienteId <= 0) {
        alert('Por favor, preencha todos os campos obrigatórios corretamente');
        return;
    }

    const contaDados = {
        id: id ? parseInt(id, 10) : 0,
        descricao: descricao,
        valor: valor,
        dataVencimento: new Date(dataVencimento + 'T00:00:00').toISOString(),
        dataPagamento: dataPagamento ? new Date(dataPagamento + 'T00:00:00').toISOString() : null,
        status: status,
        clienteId: clienteId
    };

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_BASE_URL}/api/ContasReceber/${id}` : `${API_BASE_URL}/api/ContasReceber`;

    try {
        const response = await fetchWithToken(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(contaDados)
        });

        if (!response.ok) throw new Error('Erro ao salvar conta');

        window.location.href = 'index.html';
    } catch (error) {
        console.error('Erro ao salvar:', error);
        alert('Erro ao salvar a conta a receber. Tente novamente.');
    }
}
