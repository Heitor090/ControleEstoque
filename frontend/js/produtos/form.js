const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');

const form = document.getElementById('form-produto');

async function carregarFornecedores() {
    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Fornecedores`);
        if (!response.ok) throw new Error('Erro ao carregar fornecedores');

        const fornecedores = await response.json();
        const select = document.getElementById('fornecedorId');
        select.innerHTML = '<option value="">Selecione um fornecedor</option>';

        fornecedores.forEach(fornecedor => {
            const option = document.createElement('option');
            option.value = fornecedor.id;
            option.textContent = fornecedor.nomeFantasia;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Erro ao carregar fornecedores:', error);
        alert('Erro ao carregar a lista de fornecedores');
    }
}

async function carregarProduto() {
    await carregarFornecedores();

    if (id) {
        document.getElementById('titulo-pagina').innerText = 'Editar Produto';

        try {
            const response = await fetchWithToken(`${API_BASE_URL}/api/Produtos/${id}`);
            if (!response.ok) throw new Error('Erro ao carregar produto');

            const produto = await response.json();

            document.getElementById('nome').value = produto.nome;
            document.getElementById('preco').value = produto.preco;
            document.getElementById('quantidadeEstoque').value = produto.quantidadeEstoque;
            document.getElementById('imagemUrl').value = produto.imagemUrl || '';
            document.getElementById('fornecedorId').value = produto.fornecedorId;
        } catch (error) {
            console.error('Erro ao carregar produto:', error);
            alert('Erro ao carregar os dados do produto');
        }
    }
}

form.addEventListener('submit', async (e) => {
    e.preventDefault();

    const nome = document.getElementById('nome').value.trim();
    const preco = parseFloat(document.getElementById('preco').value);
    const quantidadeEstoque = parseInt(document.getElementById('quantidadeEstoque').value, 10);
    const imagemUrl = document.getElementById('imagemUrl').value.trim();
    const fornecedorId = parseInt(document.getElementById('fornecedorId').value, 10);

    if (!nome || Number.isNaN(preco) || preco < 0 || Number.isNaN(quantidadeEstoque) || quantidadeEstoque < 0 || Number.isNaN(fornecedorId) || fornecedorId <= 0) {
        alert('Por favor, preencha todos os campos obrigatórios corretamente');
        return;
    }

    const produtoDados = {
        id: id ? parseInt(id, 10) : 0,
        nome: nome,
        preco: preco,
        quantidadeEstoque: quantidadeEstoque,
        imagemUrl: imagemUrl || null,
        fornecedorId: fornecedorId
    };

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_BASE_URL}/api/Produtos/${id}` : `${API_BASE_URL}/api/Produtos`;

    try {
        const response = await fetchWithToken(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(produtoDados)
        });

        if (!response.ok) throw new Error('Erro ao salvar produto');

        window.location.href = 'index.html';
    } catch (error) {
        console.error('Erro ao salvar:', error);
        alert('Erro ao salvar o produto. Tente novamente.');
    }
});

carregarProduto();