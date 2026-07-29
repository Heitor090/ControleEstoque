function obterCarrinho() {
    try {
        const dados = localStorage.getItem('carrinho');
        return dados ? JSON.parse(dados) : [];
    } catch (e) {
        return [];
    }
}

function salvarCarrinho(carrinho) {
    localStorage.setItem('carrinho', JSON.stringify(carrinho));
    atualizarContadorCarrinho();
}

function adicionarAoCarrinho(produto) {
    const carrinho = obterCarrinho();
    const itemExistente = carrinho.find(item => item.id === produto.id);

    if (itemExistente) {
        if (itemExistente.quantidade < produto.quantidadeEstoque) {
            itemExistente.quantidade += 1;
        } else {
            alert('Quantidade máxima em estoque atingida para este produto.');
            return;
        }
    } else {
        carrinho.push({
            id: produto.id,
            nome: produto.nome,
            preco: produto.preco,
            quantidade: 1,
            quantidadeEstoque: produto.quantidadeEstoque,
            imagemUrl: produto.imagemUrl
        });
    }

    salvarCarrinho(carrinho);
    alert(`"${produto.nome}" adicionado ao carrinho!`);
}

function alterarQuantidadeCarrinho(produtoId, delta) {
    const carrinho = obterCarrinho();
    const item = carrinho.find(i => i.id === produtoId);
    if (!item) return;

    item.quantidade += delta;
    if (item.quantidade <= 0) {
        const idx = carrinho.indexOf(item);
        carrinho.splice(idx, 1);
    } else if (item.quantidade > item.quantidadeEstoque) {
        alert('Quantidade acima do limite em estoque.');
        item.quantidade = item.quantidadeEstoque;
    }

    salvarCarrinho(carrinho);
}

function removerDoCarrinho(produtoId) {
    const carrinho = obterCarrinho().filter(item => item.id !== produtoId);
    salvarCarrinho(carrinho);
}

function limparCarrinho() {
    localStorage.removeItem('carrinho');
    atualizarContadorCarrinho();
}

function atualizarContadorCarrinho() {
    const carrinho = obterCarrinho();
    const totalItens = carrinho.reduce((total, item) => total + item.quantidade, 0);
    
    const elementosContador = document.querySelectorAll('#cart-count');
    elementosContador.forEach(el => {
        el.textContent = totalItens;
    });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', atualizarContadorCarrinho);
} else {
    atualizarContadorCarrinho();
}
