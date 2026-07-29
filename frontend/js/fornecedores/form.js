const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('form-fornecedor');
    if (form) {
        form.addEventListener('submit', salvarFornecedor);
    }
    carregarFornecedor();
});

async function carregarFornecedor() {
    if (id) {
        const titulo = document.getElementById('titulo-pagina');
        if (titulo) titulo.innerText = "Editar Fornecedor";
        
        try {
            const response = await fetchWithToken(`${API_BASE_URL}/api/Fornecedores/${id}`);
            if (!response.ok) throw new Error('Erro ao carregar fornecedor');
            
            const fornecedor = await response.json();
            
            const inputNome = document.getElementById('nomeFantasia');
            const inputCnpj = document.getElementById('cnpj');
            if (inputNome) inputNome.value = fornecedor.nomeFantasia;
            if (inputCnpj) inputCnpj.value = fornecedor.cnpj;
        } catch (error) {
            console.error("Erro ao carregar fornecedor:", error);
            alert('Erro ao carregar os dados do fornecedor');
        }
    }
}

async function salvarFornecedor(e) {
    e.preventDefault();

    const inputNome = document.getElementById('nomeFantasia');
    const inputCnpj = document.getElementById('cnpj');
    if (!inputNome || !inputCnpj) return;

    const nomeFantasia = inputNome.value.trim();
    const cnpj = inputCnpj.value.trim();

    if (!nomeFantasia || !cnpj) {
        alert('Por favor, preencha todos os campos obrigatórios');
        return;
    }

    const fornecedorDados = {
        id: id ? parseInt(id, 10) : 0,
        nomeFantasia: nomeFantasia,
        cnpj: cnpj
    };

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_BASE_URL}/api/Fornecedores/${id}` : `${API_BASE_URL}/api/Fornecedores`;

    try {
        const response = await fetchWithToken(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(fornecedorDados)
        });

        if (!response.ok) throw new Error('Erro ao salvar fornecedor');
        
        window.location.href = 'index.html';
    } catch (error) {
        console.error("Erro ao salvar:", error);
        alert('Erro ao salvar o fornecedor. Tente novamente.');
    }
}